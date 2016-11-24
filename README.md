# XamarinFormsBot
As you can see the documentation is really poor right now but I'll get to work on this as the project advances.
Most important parts of the code are commented so you can get a better glimpse of what is going on.
I strongly recommend you take a look at the Bot Framework DirectLine 3.0 documentation here: https://docs.botframework.com/en-us/restapi/directline3/#navtitle

This is a sample on how to use the new Microsoft Bot Framework on Xamarin Forms.
Now, there are basically two ways to do this:
- Embed a WebView in your Xamarin Forms page and user Bot Framework's WebChat client
- Use DirectLine 3.0 and call the Bot's API straight from your app.

We are taking the second approach as most of the Devs will want to create a personalized experience around Bots for their users. As you can see in the sample, there's not a lot effort to make, just making some HTTP request to the BOTs API and playing around with Authorization headers / watermarks.
As the project advances I'll add more advanced features to retrieve a full conversation by using the user's ID, conversation's ID, watermarks, etc.

Please feel free to send me any feedback or request features. You can reach me at jocupi@microsoft.com 
