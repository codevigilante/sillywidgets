# sillywidgets v0.3

Silly Widgets is a .NET Core web framework for AWS Lambda. <Insert punchline here>  

# synopsis

Silly Widgets is a C# web framework, using .NET Core, that runs on Amazon's Lambda platform. Its goals are:  

0) Make it easy, cheap, and fast to get a site up and going. This is relative; configuring Amazon can be a bitch. But this is more from a coding standpoint than an infrastructure one.  
A) Cheaper hosting costs for small to medium sites, because Lambda is charged only for actual running time and not by the second even if it's not doing anything.  
B) Fast and small, get in, get out, don't try to be everything, because this aligns well with goal A. Purge the unnecessary, optimize the most used.   
  
Silly Widgets is really about how A-ron would like to build sites. And since A-ron doesn't like paying bills for things he's not using, he decided to build this thing trying to take the best parts of other frameworks he's used, like Codeigniter, Wicket, ASP.NET, and Struts. And since he loves .NET C#, why not?  

Referring to myself in third person feels like an out of body experience. I'm so turned on by me.  

# getting it

# using it

Subclass `SillyProxyHandler`. `SillyProxyHandler` has a method named `Handle()` which is designed to take proxied requests from API Gateway and respond with HTML or JSON.  

# todo

* build lightening fast HTML parser
* getting and processing views (S3) with data
* update namespaces
* database interface (RDS)
* figure out how to deal with views
* figure out how to deal with database stuff

# Spinoffs

* Codeless - genericize the controller and view to allow users to accomplish everything in the HTML without having to write any code
* SillyBlog - a derived blog engine