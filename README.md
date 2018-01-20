# sillywidgets

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
* [sillywidgets version 0.6](https://www.nuget.org/packages/sillywidgets/) - the core crap
* [SillyWidgets.Utilities version 0.4.0](https://www.nuget.org/packages/SillyWidgets.Utilities/) - some useful development tools, like a development HTTP server tailored to accept Silly Sites.

# ideal development workflow

## AWS Nonsense

### Credentials & Roles & All That Shit

**Don't be lazy about this stuff, like me, and find out the hard way that there are a plethora of ne'er do wells out there with nothing better to do than take advantage of your (my) laziness. This is a one time pain in the ass that will only require management moving forward.**

The very first thing you should do is download the [AWS Command Line Interface](https://aws.amazon.com/cli/). This will help you manage all things AWS from the command line. But mainly pushing static content to S3, which is where Silly Widgets prefers to pull views from.

The very second thing you should do is create an IAM user with access to S3, and any other AWS services you'll be using. Then, create an access key for this user to make local development easier/possible, and to allow you to access your S3 resources from the command line. Once you've created an access key, you can add it to the AWS local store by running `aws configure` which will ask you for the things it wants. DON'T USE YOUR ROOT/GOD/ALL ACCESS ALL THE TIME/WHATEVER USER FOR THIS PURPOSE!!

PLEASE DO NOT PUBLISH YOUR ACCESS KEYS. THIS INCLUDES PUSHING THEM TO A PUBLIC (OR PRIVATE, REALLY) CODE REPOSITORY. Github will let you know if you've done this by accident. I'm not sure about the others, but they don't really matter, do they?

The very third thing you should do is create an IAM Role for your lambda function. This role should provide full access to S3 and any other services your lambda function will use, but only those services and nothing else. This will be the role you assign your lambda function when you're ready to set it up.

### Lambda setup tips

* The more memory you allocate to your function, the more CPU power you get.
* **"Cold" start times are a bitch**, so if you're function is called infrequently, it might be wiser to allocate more memory to cut down on startup costs. If your function is called frequently, like within the threshold of when AWS spins your function down, then you can probably get away with using less memory.

## local development

Create a directory structure that looks like this:  
  
* `<your-site-name>`
    * `lambda` or `web` or `prod` or `whatever` - where the stuff deployed to AWS Lambda will live
        * `controllers` - where you'll put your controller classes
        * `views` - where you'll put view stuff
    * `local` or `test` or `dev` - where you'll test your site locally
    * (optional) `unit` or `test` or `whatever` - where you'll do your unit testing, if you're of that sort  
  
Make a new classlib project in the `lambda` dir:

* `dotnet new classlib -f netcoreapp1.1`
* Rename the `.csproj` and `Class1.cs` if you like

Add Silly Widgets package reference in `.csproj`:

`<ItemGroup>
    <PackageReference Include="sillywidgets" Version="0.5.0" />
</ItemGroup>`

Run `dotnet restore`

Create a class that inherits from `SillyProxyApplication`

Create a controller class that inherits from `SillyController` and put it in the `controller` directory (if you like)

Create a method on that controller class called `Index` that returns an `ISillyView` and accepts an `ISillyContext` class

Inside that method, create a view `SillyView view = new SillyView();`

Add content to the view `view.Content = "Hello World";`

Return the view 'return(view);`

Back in your proxy class, register your controller `base.RegisterController("your-key", new WhateverController());`

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

* v0.7 - Jan 2018
* figure out to resolve POST
* and start thinking about sessions and cookies and shit
* vFUTURE
* move dyanmo stuff to a base model class, and how to bind these models to views
* improve loading from S3, dynamo
* improve parsing HTML
* implement markup inheritance, because it's hot
* is there a way to not use HTML but still keep the view separate? (i.e. not having to compile every time a little thing changes, or at least cache HTML so it doesn't always have to be loaded)
* abstract out SillyApplication, and think about how to build an HTTP server for Silly applications (like Tomcat for Java), or how to use in IIS
* transition to netcoreapp2.0, when this is available in Lambda
* add a CLI tool to spin up a new project to avoid creating all the scaffolding code by hand

# spinoffs, OR coming soon, OR could potentially might happen

* Codeless - genericize the controller and view to allow users to accomplish everything in the HTML without having to write any code
* SillyBlog - a derived blog engine
* SillyOMG - a help desk of sorts, or, as we used to call it, the no-help desk. Get it? DO YOU GET IT?

#### internal use only

Do not read this. Reading this will give you AIDS and cancer and cause your penis/vagina to fall off.

`dotnet pack -o ./bin/Release/ -c Release` 