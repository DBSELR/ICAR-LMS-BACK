using ICAR_LMS_BACK.Models;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;


namespace ICAR_LMS_BACK.Services
{
    public class CertificatePdfService
    {
        public byte[] GenerateCertificatePdf(
           CertificateModel data)
        {
            return Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(40);

                    page.Content().Column(col =>
                    {
                        col.Item()
                            .AlignCenter()
                            .Text("Certificate of Completion")
                            .FontSize(30)
                            .Bold();

                        col.Item()
                            .PaddingTop(40)
                            .AlignCenter()
                            .Text("This is to certify that")
                            .FontSize(18);

                        col.Item()
                            .PaddingTop(20)
                            .AlignCenter()
                            .Text(data.StudentName)
                            .FontSize(28)
                            .Bold();

                        col.Item()
                            .PaddingTop(30)
                            .AlignCenter()
                            .Text(
                                $"has successfully completed the course"
                            )
                            .FontSize(18);

                        col.Item()
                            .PaddingTop(15)
                            .AlignCenter()
                            .Text(data.CourseName)
                            .FontSize(24)
                            .Bold();

                        col.Item()
                            .PaddingTop(30)
                            .AlignCenter()
                            .Text(
                                $"Final Score : {data.FinalScore}"
                            )
                            .FontSize(18);

                        col.Item()
                            .PaddingTop(10)
                            .AlignCenter()
                            .Text(
                                $"Certificate : {data.CertificateCategory}"
                            )
                            .FontSize(18);

                        col.Item()
                            .PaddingTop(50)
                            .AlignCenter()
                            .Text(
                                $"Generated On : {DateTime.Now:dd-MM-yyyy}"
                            );
                    });
                });
            })
            .GeneratePdf();
        }
    }
}
