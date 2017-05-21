# sillywidgets

An atypical static site generator built using .NET Core 1.1. For Mac and Windows.

# synopsis

Silly Widgets is a static site generator for making client heavy web apps. It uses the concept of widgets, which are bits of reusable HTML to construct complex web sites.

# installing

## on Mac (OSx 10.11 and above)

1. Download the Mac archive from [sillywidgets.com/mac](http://sillywidgets.com/mac).
1. Unzip it to a reasonable location.
1. In Terminal, go to the directory where you unzipped the archive.
1. Run `chmod +x ./install.sh`
1. Then run `./install.sh`

Silly Widgets installs to your `/usr/local/lib` directory and creates a link in `/usr/local/bin` to the `silly` executable. This allows you to enjoy `silly` from wherever you like to develop.

## on Windows

In the works...

## build from source

1. Download dotnet CLI tools from [here](https://www.microsoft.com/net/core)
1. `git clone https://github.com/codevigilante/sillywidgets.git`
1. `cd` into silly directory
1. `dotnet build` builds the app, duh
1. `dotnet publish -c Release -r osx.10.11-x64` packages the app for deployment to a specific target, in this case, MacOS 10.11 x64

There's also scripts to publish to specific platforms. Such as `publish_mac_10.11.sh` Give one of them a shot if you're feeling lazy.

# usage

**silly new [-location <path/to/route/directory>] [-name <site-name>]** - Initialize a new silly site:
  
* <directory>
    * .silly
        * site_config.json
	* widgets
        * hello.html
    * routes
        * index.html 
    * site.json

**silly compile [-location <path/to/route/directory>]** - Checks the silly site for errors, ensuring widgets referenced exist and all assets are accounted for.  

**silly build [-location <path/to/route/directory>]** - Compiles the silly site and starts a development HTTP server.   
  
**silly deploy [-location <path/to/route/directory>]** - Compile and package the silly site for deployment    
  
# rules  

* Widgets cannot reference other widgets
* Routes can reference widgets only
* The directory structure of routes will be the exact same structure that's built

# todo v0.1

* create website, sillywidgets.com

# todo v0.2

* figure out how to pass in and resolve variables, like {{version}}
* read and send images to the client in build mode