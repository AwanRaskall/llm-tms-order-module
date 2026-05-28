using MsgReader;
using System;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace OrderModule.Application.DomainServices.EmailExtraction.Services
{
    /// <summary>
    /// Extracts plain text content from email files (.eml and .msg formats).
    /// Acts as an entry point that delegates to format-specific extractors.
    /// </summary>
    public static class ExtractTextService
    {
        /// <summary>
        /// Detects the file format by extension and routes to the appropriate extractor.
        /// </summary>
        public static string ExtractText(Stream fileStream, string fileName)
        {
            var extension = Path.GetExtension(fileName).ToLower();

            if (extension == ".eml")
                return EmlExtractor(fileStream);

            if (extension == ".msg")
                return MsgExtractor(fileStream);

            return "Unsupported file format";

        }

        /// <summary>
        /// Extracts plain text from .eml files (standard email format used by Gmail)
        /// </summary>
        public static string EmlExtractor(Stream fileStream)
        {
            var message = MsgReader.Mime.Message.Load(fileStream);
            string? text = null;

            if (message.TextBody != null)
            {
                text = Encoding.UTF8.GetString(message.TextBody.Body);
            }

            if (string.IsNullOrEmpty(text) && message.HtmlBody != null)
            {
                text = Encoding.UTF8.GetString(message.HtmlBody.Body);
                text = DeleteHtmlTags(text);
            }

            return text ?? string.Empty;
        }

        /// <summary>
        /// Extracts plain text from .msg files (Outlook proprietary format)
        /// </summary>
        public static string MsgExtractor(Stream fileStream)
        {
            using (var message = new MsgReader.Outlook.Storage.Message(fileStream))
            {
                string? text = null;

                if (message.BodyText != null)
                {
                    text = message.BodyText;
                }

                if (string.IsNullOrEmpty(text) && message.BodyHtml != null)
                {
                    text = DeleteHtmlTags(message.BodyHtml);
                }

                return text ?? string.Empty;
            }

        }

        /// <summary>
        /// Removes all HTML tags from a string using regex
        /// </summary>
        public static string DeleteHtmlTags(string html)
        {
            return Regex.Replace(html, "<.*?>", string.Empty);
        }
    }
}
