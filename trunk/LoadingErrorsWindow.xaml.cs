using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace CombatAutomationTheater
{
    /// <summary>
    /// Interaction logic for LoadingErrors.xaml
    /// </summary>
    public partial class LoadingErrorsWindow : Window
    {
        private Paragraph RichTextParagraph;
        public LoadingErrorsWindow()
        {
            InitializeComponent();
            this.RichTextParagraph = new Paragraph();
            Error_RichTextBox.Document = new FlowDocument(RichTextParagraph);
            RichTextParagraph.Inlines.Add(new Bold(new Run("Errors Seen During Loading\n\n"))
            {
                FontSize = 18,
            });
        }

        public void AddError(Tuple<String, String> loadingError, int errorNumber)
        {
            RichTextParagraph.Inlines.Add(new Bold(new Run(errorNumber+" - File: "+loadingError.Item1))
            {
                Foreground = Brushes.Black,
                FontSize = 14,
            });
            RichTextParagraph.Inlines.Add(new Bold(new Run("\nError: "+loadingError.Item2+"\n\n"))
            {
                Foreground = Brushes.Black,
                FontSize = 10,
            });
        }

        internal void SetErrors(List<Tuple<string, string>> LoadingErrors)
        {
            int curIndex = 1;
            foreach (var cur in LoadingErrors)
            {
                AddError(cur, curIndex);
                curIndex++;
            }
        }
    }
}
