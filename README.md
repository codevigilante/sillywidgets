# sillywidgets

The Atypical Web App Framework, Or, The Atypical Static Site Generator

# synopsis

Silly Widgets is a static site generator for making client heavy web apps. It uses the concept of widgets, which are bits of reusable HTML to construct complex web sites.

# installing

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

* figure out how to deploy and distribute (this thing)
* create website, sillywidgets.com

# todo v0.2

* figure out how to pass in and resolve variables, like {{version}}
* read and send images to the client
