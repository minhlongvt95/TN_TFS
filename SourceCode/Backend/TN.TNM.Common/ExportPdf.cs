using System;
using System.IO;
using System.Threading.Tasks;
using DinkToPdf;
using iTextSharp.text;
using iTextSharp.text.pdf;
using SelectPdf;
//using Microsoft.Extensions.Hosting;

//using Microsoft.AspNetCore.Hosting;
//using Microsoft.AspNetCore.Mvc;

namespace TN.TNM.Common
{
    public class ExportPdf
    {
        static SynchronizedConverter converter;
        static byte[] pdf;

        public static byte[] HtmlToPdfExport(string xhtml, string filepath,PdfPageSize pageSize, PdfPageOrientation pdfPageOrientation,string HtmlFooter)
        {
            // instantiate the html to pdf converter
            HtmlToPdf _converter = new HtmlToPdf();
            _converter.Options.PdfPageSize = pageSize; 
            _converter.Options.PdfPageOrientation = pdfPageOrientation;
            _converter.Options.DisplayFooter = true;
            _converter.Footer.DisplayOnFirstPage = true;
            _converter.Footer.DisplayOnOddPages = true;
            _converter.Footer.DisplayOnEvenPages = true;
            _converter.Footer.Height = Convert.ToInt32(50);
            if (!string.IsNullOrEmpty(HtmlFooter))
            {
                PdfHtmlSection footerHtml = new PdfHtmlSection(HtmlFooter, string.Empty);
                footerHtml.AutoFitHeight = HtmlToPdfPageFitMode.AutoFit;
                _converter.Footer.Add(footerHtml);
            }
            SelectPdf.PdfDocument doc = _converter.ConvertHtmlString(xhtml);

            // save pdf document
            doc.Save(filepath);

            // close pdf document
            doc.Close();
            var contentPdf = File.ReadAllBytes(filepath);
            return contentPdf;
        }

        public static byte[] ExportBytePdf(string xhtml, string css, int _orientation)
        {
            converter = new SynchronizedConverter(new PdfTools());
            var pathcss = Path.Combine(Directory.GetCurrentDirectory(), "Template", "PdfTemplate", "assets", "bootstrap.min.css");
            try
            {
                converter.PhaseChanged += Converter_PhaseChanged;
                converter.ProgressChanged += Converter_ProgressChanged;
                converter.Finished += Converter_Finished;
                converter.Warning += Converter_Warning;
                converter.Error += Converter_Error;

                var doc = new HtmlToPdfDocument()
                {
                    GlobalSettings = {
                    ColorMode = ColorMode.Color,
                    Orientation = _orientation==0? Orientation.Landscape : Orientation.Portrait,
                    PaperSize = PaperKind.A4,

                },
                    Objects = {
                    new ObjectSettings() {
                        PagesCount = true,
                        HtmlContent = "UIFFF",
                        WebSettings = {  DefaultEncoding = "utf-8" , UserStyleSheet =   pathcss },
                        //HeaderSettings = { FontSize = 9, Right = "Page [page] of [toPage]", Line = true },
                        //FooterSettings = { FontSize = 9, Right = "Page [page] of [toPage]"}
                    }
                }

                };

                //pdf= await Action(doc) ///Task.Run(() => Action(doc)).Result;
                //Task<byte[]> task = Action(doc);
                //task.Wait();

                //pdf = task.Result;
                //task.Dispose();
                var contentPdf = converter.Convert(doc);
                return contentPdf;


            }
            catch (Exception ex)
            {

                return pdf;
                converter = null;
            }
            finally
            {
                converter = null;
            }


        }

        private static async Task<byte[]> Action(HtmlToPdfDocument doc)
        {
            byte[] pdf;
            try
            {
                pdf = converter.Convert(doc);

            }
            catch (TaskSchedulerException ex)
            {
                throw;

            }

            return pdf;
        }
        public static string GetStringHtml(string fileNameHtml)
        {
            string path = Path.GetFullPath("~/Template/PdfTemplate").Replace("~\\", "") + "//" + fileNameHtml;
            var result = File.ReadAllText(path);
            return result;
        }
        public static string GetstrgCss(string css)
        {
            string path = Path.GetFullPath("~/Template/PdfTemplate/assets").Replace("~\\", "") + "//" + css;
            var result = File.ReadAllText(path);
            return result;
        }
        public class UnicodeFontProvider : FontFactoryImp
        {
            /// <summary>
            /// Provides a font with BaseFont.IDENTITY_H encoding.
            /// </summary>
            public override Font GetFont(string fontname, string encoding, bool embedded, float size, int style, BaseColor color, bool cached)
            {
                if (string.IsNullOrWhiteSpace(fontname))
                {
                    //TODO: set a default font
                }
                return FontFactory.GetFont(fontname, BaseFont.IDENTITY_H, BaseFont.EMBEDDED, size, style, color);
            }
        }
        private static void Converter_Error(object sender, DinkToPdf.EventDefinitions.ErrorArgs e)
        {
            Console.WriteLine("[ERROR] {0}", e.Message);
        }

        private static void Converter_Warning(object sender, DinkToPdf.EventDefinitions.WarningArgs e)
        {
            Console.WriteLine("[WARN] {0}", e.Message);
        }

        private static void Converter_Finished(object sender, DinkToPdf.EventDefinitions.FinishedArgs e)
        {
            Console.WriteLine("Conversion {0} ", e.Success ? "successful" : "unsucessful");

        }

        private static void Converter_ProgressChanged(object sender, DinkToPdf.EventDefinitions.ProgressChangedArgs e)
        {
            Console.WriteLine("Progress changed {0}", e.Description);
        }

        private static void Converter_PhaseChanged(object sender, DinkToPdf.EventDefinitions.PhaseChangedArgs e)
        {
            Console.WriteLine("Phase changed {0} - {1}", e.CurrentPhase, e.Description);
        }
    }
}
