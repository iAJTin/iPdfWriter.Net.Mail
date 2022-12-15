﻿
using System;
using System.Drawing;
using System.Threading;
using System.Threading.Tasks;

using iTin.Core.ComponentModel;
using iTin.Mail.Smtp.Net;
using iTin.Utilities.Pdf.Design.Image;

using iPdfWriter.Abstractions.Writer.Operations.Actions;
using iPdfWriter.Operations.Replace;
using iPdfWriter.Operations.Replace.Replacement.Text;

namespace iPdfWriter.Net.Mail.ConsoleApp.Code
{
    using ComponentModel.Helpers;

    /// <summary>
    /// Shows how to send pdf output result by email with <strong>Mailtrap</strong> (fake SMTP provider service) asynchronously.
    /// <para>
    /// For more information, please see <a href="https://mailtrap.io/home">https://mailtrap.io/home</a>.
    /// </para>
    /// </summary>
    internal static class Sample02
    {
        public static async Task GenerateAsync(CancellationToken cancellationToken = default)
        {
            var pdfCreationResult = await BuildPdfInput().CreateResultAsync(cancellationToken: cancellationToken);
            if (!pdfCreationResult.Success)
            {
                return;
            }

            var sendResult = await pdfCreationResult.Result.Action(new SendMailAsync
                {
                    AttachedFilename = "Sample-01",
                    FromDisplayName = "Fernando",
                    FromAddress = "fdo.garcia.vega@gmail.com",
                    Settings = new SmtpMailSettings
                    {
                        Credential = new SmtpCredential
                        {
                            Port = 2525,
                            UseSsl = true,
                            Host = SmtpMail.MailtrapSmtpHost,
                            Email = "fdo.garcia.vega@gmail.com",
                            UserName = "668aa4b2008e20",
                            Password = "b374b7eb13c0f8"
                        },
                        Templates = new TemplateSettings
                        {
                            IsBodyHtml = true,
                            BodyTemplate = "Hey!!",
                            SubjectTemplate = "Test > pdf file"
                        },
                        Recipients = new RecipientsSettings
                        {
                            ToAddresses = new[] { "fernando.garcia@col.vueling.com" }
                        },
                        Attachments = new[]
                        {
                            "~/Resources/Sample-01/Images/bar-chart.png",
                            "~/Resources/Sample-01/Images/image-1.jpg"
                        }
                    }
                },
                cancellationToken);

            if (!sendResult.Success)
            {
                Console.WriteLine(sendResult.Errors.AsMessages().ToStringBuilder());
            }
        }


        private static PdfInput BuildPdfInput()
        {
            var doc = new PdfInput
            {
                AutoUpdateChanges = true,
                Input = "~/Resources/Sample-01/file-sample.pdf"
            };

            // report title
            doc.Replace(new ReplaceText(
                    new WithTextObject
                    {
                        Text = "#TITLE#",
                        NewText = "Lorem ipsum",
                        Offset = PointF.Empty,
                        Style = StylesHelper.Sample01.TextStylesTable["ReportTitle"],
                        ReplaceOptions = ReplaceTextOptions.AccordingToMargins
                    }))
                // bar-chart image
                .Replace(new ReplaceText(
                    new WithImageObject
                    {
                        Text = "#BAR-CHART#",
                        Offset = PointF.Empty,
                        Style = StylesHelper.Sample01.ImagesStylesTable["Default"],
                        ReplaceOptions = ReplaceTextOptions.Default,
                        Image = PdfImage.FromFile("~Resources/Sample-01/Images/bar-chart.png")
                    }))
                // image
                .Replace(new ReplaceText(
                    new WithImageObject
                    {
                        Text = "#IMAGE1#",
                        Offset = PointF.Empty,
                        Style = StylesHelper.Sample01.ImagesStylesTable["Center"],
                        ReplaceOptions = ReplaceTextOptions.AccordingToMargins,
                        Image = PdfImage.FromFile("~/Resources/Sample-01/Images/image-1.jpg")
                    }));

            return doc;
        }
    }
}
