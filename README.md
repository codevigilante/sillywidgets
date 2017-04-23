# sillywidgets

The Atypical Web App Framework, Or, The Atypical Static Site Generator

# synopsis

Silly Widgets is a static site generator for making client heavy web apps. It uses the concept of widgets, which are bits of reusable HTML to construct complex web sites.

# installing

# usage

**silly new [site-name]** - creates a new [site-name] directory (default is 'newsite') with the structure:

* site-name
    * .silly
        * site_config.json
	* widgets
    * routes
        * index.html 
    * site.json

**silly compile [-location <path/to/route/directory>]** - consumes the site.json and checks for errors  

    -location <path/to/route/directory> compiles site.json at the location  

**silly build [-location <path/to/route/directory>]** - starts a local web server and builds the html based on site.json

     -location <path/to/route/directory> the root location where the server should run    
  
**silly deploy** - builds up all the static content for pushing to server    
  
# rules

* Widgets cannot reference other widgets
* Routes can reference widgets only
* The directory structure of routes will be the exact same structure that's built

# todo v0.1

* implement the deploy process
* implement the new directive
* figure out how to deploy and distribute
* create website, sillywidgets.com

# todo v0.2

* figure out how to pass in and resolve variables, like {{version}}
* read and send images to the client
