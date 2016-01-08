Trim Copy
=========

Trim Copy is a Visual Studio extension to copy selected code block to the clipboard trimming leading white spaces while keeping indentation structure. It allows you to copy the code in text editor of Visual Studio, trim unnecessary white spaces and paste it into a document in Markdown or other format by one step.

##Requirements

 * Visual Studio 2015

##Development

To run an extension in experimental instance of Visual Studio 2015, you need the following settings in __Debug__ pane of project properties.

 - In __Start Action__, select __Start external program__ and input `C:\Program Files (x86)\Microsoft Visual Studio 14.0\Common7\IDE\devenv.exe` (this path depends on the install path of Visual Studio off course).
 - In __Start Options__, fill __Command line arguments__ with `/rootsuffix Exp`.

These are default for Extensibility VSIX project and written in csproj.user file but that file is not included in the repository in principle.

##History

Ver 1.0 2016-1-6

 - Initial release

##License

 - MIT License