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

Use NUGET, the preferred package manager of .NET Core stuffs.

Include in your `.csproj` file:
* [sillywidgets version 0.3.0](https://www.nuget.org/packages/sillywidgets/) - the core crap
* [SillyWidgets.Utilities version 0.3.0](https://www.nuget.org/packages/SillyWidgets.Utilities/) - some useful development tools, like a development HTTP server tailored to accept Silly Sites.

# ideal development workflow

Create a directory structure that looks like this:  
  
* `<your-site-name>`
    * `lambda` or `web` or `prod` or `whatever` - where the stuff deployed to AWS Lambda will live
        * `controllers` - where you'll put your controller classes
        * `views` - where you'll put view stuff
    * `local` or `test` or `dev` - where you'll test your site locally
    * (optional) `unit` or `test` or `whatever` - where you'll do your unit testing, if your of that sort  
  
Make a new classlib project in the `lambda` dir:

* `dotnet new lasslib -f netcoreapp1.1`
* Rename the `.csproj` and `Class1.cs` if you like

Add Silly Widgets package reference in `.csproj`:

`<ItemGroup>
    <PackageReference Include="sillywidgets" Version="0.3.0" />
</ItemGroup>`

Run `dotnet restore`

Create a class that inherits from `SillyProxyApplication`

Create a controller class that inherits from `SillyController` and put it in the `controller` directory (if you like)

Create a method on that controller class called `Index` that returns an `ISillyView` and accepts an `ISillyContext` class

Inside that method, create a view `SillyView view = new SillyView();`

Add content to the view `view.Content = "Hello World";`

Return the view 'return(view);`

Back in your proxy class, register your controller `base.RegisterController("your-key", typeof(your-controller-type));`

Then setup the route that applies to this controller `GET("root", "/", "root", "Index");`

`dotnet build` and fix errors

Switch to your `local` directory and create the development server `dotnet new console -f netcoreapp1.1`

Add a project reference to the `lambda` project so you can use it:

`<ItemGroup>
    <ProjectReference Include="..\lambda\site.lambda.csproj" />
</ItemGroup>`

Run `dotnet restore`

Create the development server and start:

`SillyProxyApplication site = new SillyWidgetsProxy();
SillySiteServer testServer = new SillySiteServer(site);

Task server = testServer.Start();

server.Wait();`

Run `dotnet run` and fix any problems, probably missing some `using` directives

When all goes well, open a browser and point it at 127.0.0.1:7575 and there's your Goddamn site!

# deploying

Create a lambda package `dotnet lambda package <ZIP FILENAME> -c Release -f netcoreapp1.1`

A zip file was created that contains your deployment to upload to Lambda

Create a Lambda function, not using any blueprint (follow the AWS docs)

Upload your zip file. The Handler name will be `<assembly(.dll)::namespace.classname::Handler>, where Handler is the actual method name in SillyProxyApplication that will process all incoming requests and distribute to your controllers

Setup and API Gateway as a proxy and point it at your Lambda function

These directions are extremely general and vague, but Amazon has pretty good docs to help you through permissions and security and DNS all that crap 

# todo

* v0.4
* Add support for reading from S3 and make this the standard way of handling static resources
* Build and launch sillywidgets.com placeholder
* v0.4
* transition to netcoreapp2.0
* make dealing with local views and resources (like css) better (seamless)
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

# spinoffs, OR coming soon, OR could potentially might happen

* Codeless - genericize the controller and view to allow users to accomplish everything in the HTML without having to write any code
* SillyBlog - a derived blog engine