# sillywidgets

The Atypical Web App Framework, Or, The Atypical Static Site Generator

# synopsis

# installing

# usage

**silly new [site-name]** - creates a new [site-name] directory (default is 'newsite') with the structure:

* site-name
    * .silly
        * site_config.json
	* widgets
    * routes
    * assets
    * site.json

**silly compile [-location <path/to/site/directory>]** - consumes the site.json and checks for errors  

    -location <path/to/site/directory> compiles site.json at the location  

**silly build [-location <path/to/site/directory>]** - starts a local web server and builds the html based on site.json

     -location <path/to/site/directory> the root location where the server should run    
  
**silly deploy [—minify]** - builds up all the static content for pushing to server    

     -minify compresses all files before deploying  
  
# why

# rules

* Widgets cannot reference other widgets
* Routes can reference widgets only
* The directory structure of routes will be the exact same structure that's built
