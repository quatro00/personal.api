using HtmlAgilityPack;

namespace Farmacia.UI.Utils
{
    public class HtmlElement
    {
        public string id { get; set; }
        public string element { get; set; }
        public string elementValue { get; set; }
    }
    public class Html
    {
        public string Path { get; set; }
        public Html(string Path)
        {
            this.Path = Path;
        }

        //methods
        public string Parse()
        {
            string htmlContent = "";
            try
            {
                htmlContent = File.ReadAllText(Path);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }

            return htmlContent;
        }
        public string Parse(List<HtmlElement> htmlElements)
        {
            string htmlContent = "";
            try
            {
                if (htmlElements.Count() == 0) throw new Exception("Empty list");
                htmlContent = insertElement(htmlElements);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }

            return htmlContent;
        }

        public string insertElement(List<HtmlElement> htmlElements)
        {
            string html = "";
            try
            {
                HtmlDocument htmlDocument = new HtmlDocument();
                StringWriter stringWriter = new StringWriter();
                htmlDocument.Load(Path);

                for (int i = 0; i < htmlElements.Count(); i++)
                {
                    HtmlNode node = htmlDocument.GetElementbyId(htmlElements[i].id);
                    if (node != null)
                    {
                        node.SetAttributeValue(htmlElements[i].element, htmlElements[i].elementValue);
                        htmlDocument.Save(stringWriter);
                        //htmlDocument.Save(Path);
                    }
                }
                html = stringWriter.ToString();
            }
            catch (Exception)
            {
                throw;
            }

            return html;
        }
    }
}
