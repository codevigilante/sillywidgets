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
    * deploy
	    * assets
	        * css
		    * js
		    * img
    * site.json

**silly build [—continuous] [-location <path/to/site/directory>]** - renders the static content  

     -continuous re-renders on change  
     -location <path/to/site/directory> renders the site located at the path    
  
**silly deploy [—minify]** - pushes the site to the server  

     -minify compresses all files before deploying  
  
# why


