using System;
using System.Collections.Generic;
using System.IO;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Win32;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.Processing.Processors.Transforms;
using Image = SixLabors.ImageSharp.Image;

namespace JpgifierDesktop
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            if (openFileDialog.ShowDialog() == true)
            {

                var inputBytes = await File.ReadAllBytesAsync(openFileDialog.FileName);

                int repetitions = int.Parse(RepetitionsBox.Text);
                var quality = int.Parse(QualityText.Text);
                var resize = ResizeCheckbox.IsChecked ?? false;
                byte[] outputBytes = new byte[0];
                for (int i = 0; i < repetitions; i++)
                {
                    outputBytes = await CompressPic(inputBytes, quality, resize);
                }


                SaveFileDialog saveFileDialog = new SaveFileDialog
                {
                    AddExtension = true, 
                    DefaultExt = "jpg"
                };

                var showDiag = saveFileDialog.ShowDialog();
                if (showDiag.HasValue && showDiag.Value)
                {
                    await File.WriteAllBytesAsync(saveFileDialog.FileName, outputBytes);
                }

                MessageBox.Show("JPGification complete");


                ////using FileStream fs = File.OpenRead(openFileDialog.FileName);
                //string baseFile = openFileDialog.FileName;
                //string lastFile = openFileDialog.FileName;

                ////byte[] lastBytes;
                //for (int i = 0; i < repetitions; i++)
                //{

                //    using Image image = Image.Load(lastFile);



                //    var width = image.Width;
                //    var height = image.Height;

                //    image.Mutate(x => x.Resize(width - 1, height - 1, KnownResamplers.Lanczos3));

                //    string newFile = $"{baseFile}.{i}.jpg";

                //    using var saveStream = new MemoryStream();

                //    //image.SaveAsJpeg(newFile, new JpegEncoder() { Quality = quality });

                //    //image.SaveAsJpeg(saveStream, new JpegEncoder() { Quality = quality });

                //    //lastBytes = saveStream.ToArray();

                //    lastFile = newFile;

                //}


            }
        }


        public async Task<byte[]> CompressPic(byte[] input, int quality, bool resize)
        {
            await using var inputStream = new MemoryStream(input);
            
            using Image image = await Image.LoadAsync(inputStream);
            
            if (resize)
            {
                var width = image.Width;
                var height = image.Height;

                image.Mutate(x => x.Resize(width - 1, height - 1, KnownResamplers.Lanczos3));
            }

            byte[] output;

            await using var ms = new MemoryStream();

            await image.SaveAsJpegAsync(ms, new JpegEncoder() { Quality = quality });

            ms.Seek(0, SeekOrigin.Begin);

            output = ms.ToArray();

            return output;
        }
    }
}
