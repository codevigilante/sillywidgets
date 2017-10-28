# sillywidgets v0.3

Silly Widgets is a C# .NET Core web framework for AWS Lambda. <Insert punchline here>  

# synopsis

Silly Widgets is a C# web framework, using .NET Core, that runs on Amazon's Lambda platform. Its goals are:  

1. Make it easy, cheap, and fast to get a site up and going. This is relative; configuring Amazon can be a bitch. But this is more from a coding standpoint than an infrastructure one.  
1. Cheaper hosting costs for small to medium sites, because Lambda is charged only for actual running time* and not by the second even if it's not doing anything.  
1. Fast and small, get in, get out, don't try to be everything. Because this aligns well with goal A. Purge the unnecessary, optimize the most used.   
  
Silly Widgets is really about how A-ron would like to build sites. And since A-ron doesn't like paying bills for things he's not using, he decided to build this thing trying to take the best parts of other frameworks he's used, like Codeigniter, Wicket, ASP.NET, and Struts. And since he loves** .NET C#, why not?  

Referring to myself in third person feels like an out of body experience. I'm so turned on by me.

\* This is Amazon's claim, not mine. Who knows if they're juicing the gas or adding a little "extra" here and there. Trust, I guess, until some really smart fucker gets pissed off enough to pull the curtains back. And you know that's how Amazon is going to crumble. One disgruntled asshole exposing all their dirty secrets.

** "Love" is a strong word here. It's not like I'm jacking off to .NET C# or pretending it's my girlfriend or anything. Pfff, I'm totally not a weirdo.

# getting it

# using it

Subclass `SillyProxyHandler`. `SillyProxyHandler` has a method named `Handle()` which is designed to take proxied requests from API Gateway and respond with HTML or JSON.  

More stuff on the way.  

# todo

* make release v0.3
* Build and launch sillywidgets.com placeholder
* v0.4
* update namespaces
* getting and processing views from S3
* dealing with widgets
* make release v0.4
* update sillywidgets.com
* v0.5
* database operations (RDS)
* binding database data to views
* make release v0.5
* update sillywidgets.com

# Spinoffs

* Codeless - genericize the controller and view to allow users to accomplish everything in the HTML without having to write any code
* SillyBlog - a derived blog engine