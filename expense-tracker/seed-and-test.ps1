$base = "http://localhost:5039"

# 1. Login
$login = Invoke-RestMethod -Method Post -Uri "$base/api/auth/login" -ContentType "application/json" `
    -Body '{"email":"sangita@test.com","password":"Test@123"}'
$h = @{ Authorization = "Bearer $($login.token)" }
Write-Host "Logged in OK" -ForegroundColor Green

# 2. Find existing categories (from the category list, and from existing expenses)
$cats = @{}
try {
    $existing = Invoke-RestMethod -Uri "$base/api/categories" -Headers $h
    $catList = if ($existing.items) { $existing.items } else { $existing }
    foreach ($c in $catList) { if ($c.name) { $cats[$c.name] = $c.id } }
} catch { Write-Host "Could not list categories (ok): $($_.Exception.Message)" -ForegroundColor Yellow }

$all = Invoke-RestMethod -Uri "$base/api/expenses?pageSize=100" -Headers $h
foreach ($i in $all.items) { if ($i.categoryName) { $cats[$i.categoryName] = $i.categoryId } }

# 3. Create only the categories that are missing
foreach ($name in "Food", "Travel", "Rent", "Salary") {
    if (-not $cats.ContainsKey($name)) {
        try {
            $c = Invoke-RestMethod -Method Post -Uri "$base/api/categories" -Headers $h `
                -ContentType "application/json" -Body (@{ name = $name } | ConvertTo-Json)
            $cats[$name] = $c.id
        } catch { Write-Host "Create category $name failed: $($_.ErrorDetails.Message)" -ForegroundColor Red }
    }
    Write-Host "Category $name -> id $($cats[$name])"
}

# 4. Add expenses (skip ones that already exist)
$rows = @(
    @{ amount = 250.50;   description = "Lunch";          expenseDate = "2026-10-14"; isIncome = $false; categoryId = $cats["Food"] },
    @{ amount = 120.00;   description = "Coffee";         expenseDate = "2026-10-13"; isIncome = $false; categoryId = $cats["Food"] },
    @{ amount = 900.00;   description = "Groceries";      expenseDate = "2026-10-10"; isIncome = $false; categoryId = $cats["Food"] },
    @{ amount = 1800.00;  description = "Train tickets";  expenseDate = "2026-10-08"; isIncome = $false; categoryId = $cats["Travel"] },
    @{ amount = 15000.00; description = "October rent";   expenseDate = "2026-10-01"; isIncome = $false; categoryId = $cats["Rent"] },
    @{ amount = 60000.00; description = "October salary"; expenseDate = "2026-10-01"; isIncome = $true;  categoryId = $cats["Salary"] }
)
$created = @()
foreach ($r in $rows) {
    $found = $all.items | Where-Object { $_.description -eq $r.description } | Select-Object -First 1
    if ($found) {
        Write-Host "Skipped (already exists) id $($found.id): $($r.description)"
        $created += $found
        continue
    }
    try {
        $e = Invoke-RestMethod -Method Post -Uri "$base/api/expenses" -Headers $h `
            -ContentType "application/json" -Body ($r | ConvertTo-Json)
        $created += $e
        Write-Host "Added expense id $($e.id): $($r.description)"
    } catch {
        Write-Host "Add '$($r.description)' failed: $($_.ErrorDetails.Message)" -ForegroundColor Red
    }
}

# 5. List with paging
Write-Host "`n--- GET list, pageSize=3 ---" -ForegroundColor Cyan
$list = Invoke-RestMethod -Uri "$base/api/expenses?page=1&pageSize=3" -Headers $h
$list | ConvertTo-Json -Depth 4
Write-Host "totalCount = $($list.totalCount) (expected 6 or more)"

# 6. Filter by category
if ($cats["Food"]) {
    Write-Host "`n--- GET filter categoryId=$($cats['Food']) (Food) ---" -ForegroundColor Cyan
    Invoke-RestMethod -Uri "$base/api/expenses?categoryId=$($cats['Food'])" -Headers $h | ConvertTo-Json -Depth 4
}

# 7. Filter by date range
Write-Host "`n--- GET filter 2026-10-08 to 2026-10-14 ---" -ForegroundColor Cyan
Invoke-RestMethod -Uri "$base/api/expenses?from=2026-10-08&to=2026-10-14" -Headers $h | ConvertTo-Json -Depth 4

# 8. Get, update, delete (only if the adds worked)
if ($created.Count -ge 2 -and $created[0].id -and $created[1].id) {
    $id = $created[0].id
    Write-Host "`n--- GET by id $id ---" -ForegroundColor Cyan
    Invoke-RestMethod -Uri "$base/api/expenses/$id" -Headers $h | ConvertTo-Json

    Write-Host "`n--- PUT id $id (amount to 275) ---" -ForegroundColor Cyan
    $upd = @{ amount = 275.00; description = "Lunch (updated)"; expenseDate = "2026-10-14"; isIncome = $false; categoryId = $cats["Food"] }
    try {
        Invoke-RestMethod -Method Put -Uri "$base/api/expenses/$id" -Headers $h -ContentType "application/json" -Body ($upd | ConvertTo-Json)
        Invoke-RestMethod -Uri "$base/api/expenses/$id" -Headers $h | ConvertTo-Json
    } catch { Write-Host "PUT failed: $($_.ErrorDetails.Message)" -ForegroundColor Red }

    $delId = $created[1].id
    Write-Host "`n--- DELETE id $delId ---" -ForegroundColor Cyan
    try { Invoke-RestMethod -Method Delete -Uri "$base/api/expenses/$delId" -Headers $h }
    catch { Write-Host "DELETE failed: $($_.ErrorDetails.Message)" -ForegroundColor Red }
    try {
        Invoke-RestMethod -Uri "$base/api/expenses/$delId" -Headers $h | Out-Null
        Write-Host "UNEXPECTED: expense still exists" -ForegroundColor Red
    } catch { Write-Host "Got 404 after delete - correct" -ForegroundColor Green }
} else {
    Write-Host "`nSkipping get/update/delete because some expenses were not created. See red errors above." -ForegroundColor Yellow
}

# 9. No token
Write-Host "`n--- GET without token ---" -ForegroundColor Cyan
try {
    Invoke-RestMethod -Uri "$base/api/expenses" | Out-Null
    Write-Host "UNEXPECTED: call succeeded without token" -ForegroundColor Red
} catch { Write-Host "Got 401 without token - correct" -ForegroundColor Green }

Write-Host "`nAll checks finished." -ForegroundColor Green