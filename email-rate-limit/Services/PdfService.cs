using QuestPDF.Fluent;
using QuestPDF.Helpers;

namespace EmailRateLimitApi.Services;

public class PdfService
{
    public byte[] CreateReport(string name)
    {
        var rows = new[]
        {
            (Id: 1, Name: "Asha Patil", Dept: "IT", Salary: 50000),
            (Id: 2, Name: "Rahul Sharma", Dept: "HR", Salary: 42000),
            (Id: 3, Name: "Neha Joshi", Dept: "Finance", Salary: 55000)
        };

        return Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(30);

                page.Header().Column(col =>
                {
                    col.Item().Text("Employee Report").FontSize(20).Bold();
                    col.Item().Text($"Prepared for {name} on {DateTime.Now:dd MMM yyyy}");
                });

                page.Content().PaddingVertical(10).Table(table =>
                {
                    table.ColumnsDefinition(c =>
                    {
                        c.ConstantColumn(40);
                        c.RelativeColumn();
                        c.RelativeColumn();
                        c.RelativeColumn();
                    });

                    table.Header(h =>
                    {
                        h.Cell().Text("Id").Bold();
                        h.Cell().Text("Name").Bold();
                        h.Cell().Text("Department").Bold();
                        h.Cell().Text("Salary").Bold();
                    });

                    foreach (var r in rows)
                    {
                        table.Cell().Text(r.Id.ToString());
                        table.Cell().Text(r.Name);
                        table.Cell().Text(r.Dept);
                        table.Cell().Text(r.Salary.ToString("N0"));
                    }
                });
            });
        }).GeneratePdf();
    }
}