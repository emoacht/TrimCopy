using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Media;
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
using Microsoft.Win32;

using TrimCopy.Models;

namespace TrimCopy.Icon
{
	public partial class MainWindow : Window
	{
		public MainWindow()
		{
			InitializeComponent();

			ScaleSlider.Value = ImageContent.Width;

			Format();
		}

		#region Format

		private void Format()
		{
			const string inputFilePath = @"c:\work\input.txt";
			const string outputFilePath = @"c:\work\output.txt";

			if (!File.Exists(inputFilePath))
				return;

			var input = File.ReadAllText(inputFilePath);

			var formatted = StringFormatter.Format(input, 4, 0, true, false, NewLineType.CrLf);

			File.WriteAllText(outputFilePath, formatted);

			Debug.WriteLine("Formatted!");
		}

		#endregion

		#region Icon

		public double ImageLength
		{
			get { return (double)GetValue(ImageLengthProperty); }
			set { SetValue(ImageLengthProperty, value); }
		}
		public static readonly DependencyProperty ImageLengthProperty =
			DependencyProperty.Register(
				"ImageLength",
				typeof(double),
				typeof(MainWindow),
				new PropertyMetadata(256D));

		private string _fileName = "Icon.png";
		private string _folderPath;

		public void SaveImage()
		{
			var sfd = new SaveFileDialog
			{
				FileName = _fileName,
				Filter = "*(.png)|*.png",
				InitialDirectory = _folderPath ?? Environment.GetFolderPath(Environment.SpecialFolder.Desktop)
			};

			if (sfd.ShowDialog() != true)
				return;

			_fileName = Path.GetFileName(sfd.FileName);
			_folderPath = Path.GetDirectoryName(sfd.FileName);

			FrameworkElementImage.SaveImage(ImageGrid, sfd.FileName, ImageLength, ImageLength);

			SystemSounds.Asterisk.Play();
		}

		#endregion
	}
}