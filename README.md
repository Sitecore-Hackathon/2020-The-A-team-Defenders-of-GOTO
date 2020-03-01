# Documentation

This readme file details how to install and use the **GOTO Usergroup site**.

## Summary

**Category:** Sitecore Meetup Website

We have created the GOTO Usergroup site, which is a rough draft of a site to for usergroups to be formed, and organize events. 
The site is implemented with the use of Sitecore Forms, List Manager, XConnect, EXM, Identity Server, Conditional renderings and generous amounts of front-end duct tape.

## Pre-requisites

The module is installed on Sitecore 9.3. It uses Unicorn to provide the necessary data, structure and users to get the site up and running.

## Installation

1. Navigate to .\src\Project\Website\GOTO-Usergroup.Project.Website\App_Config\Include\GOTO-Usergroup\Project\Website\Environment.config and add the path to your local checkout\src folder.
2. Navigate to .\src\Project\Website\GOTO-Usergroup.Project.Website\Properties\PublishProfiles\Local.pubxml and change publishUrl to the path of your local Sitecore 9.3 installation
3. Navigate to .\src\Project\Website\GOTO-Usergroup.Project.Website\App_Config\Include\GOTO-Usergroup\Project\Website\Sites.config and change the hostname and targethostname to match your local site.
4. Open the Visual Studio solution and do a full nuget restore and rebuild
5. Open your favorite browser and navigate to https://[yoursite]/unicorn.aspx and do a full sync
6. Go to the root of the site and try it out!

## Usage

The first thing you do when you open the module is register. Once you've entered your name, email and password, you're ready to log in. If you're lucky you will even receive a welcome mail (it works every time with our internal mailserver, but after switching to sendgrid for the delivery, most of it never ends up in our inboxes).

### Joining and Leaving

Once you've registered you will be able to join Usergroups and Events. Simply navigate to the one you wish to join and click the join button. After joining you will have the option to leave if you regret. This is setup with a custom conditional rendering which checks your memberstatus in XConnect.
The Sitecore CMS is just too darn fast, so joining and leaving actually does a thread sleep in order for XConnect to catch up. Otherwise the buttons and lists aren't updated correctly and would need a manual page refresh.

### Creating a usergroup

If the already existing usergroups does not meet your desires, you're free to create your own. At the time being, no validation workflows has been added to keep users from creating silly usergroups. But hey - it's Sitecore developers, who would do such a thing? ...
Just navigate to the UserGroups page and Create a New one, adding a title, description and a short name. You will automatically be added to the Member and Organizer lists of the user group.

### Creating an event

In order to create an event you navigate to a Usergroup. If you're an organizer you will see the Create New option, and add a Host, Title, Description, Date and Address. Once it's created you will automatically be signed up for the event yourself, and others can see it and sign up.

### The bugs
Unfortunately we have been experiencing a few bugs which we have not been able to iron out before deadline.
1. As mentioned earlier, we're having issues with EXM sending messages through Sendgrid - at least to our accounts. We can see that Sendgrid receives the messages.
2. The forms are having issues with several special case characters. If you want to be certain of them not failing, use only letters, digits and spaces.
3. We have created a couple of custom Form fields in order to maintain the Sitecore context, in a perfect world this would have been handled differently.

## The limitations
Given it's a 24 hour Hackathon, and we started from scratch, of course it's a limited site. Our initial brainstorm resulted in many great ideas, such as 
- The possibility of sharing inspiration and content across different usergroups and events. To facilitate better and more frequent events. 
- Interest in subjects and willingness of miles to travel, enabling the site to push events to the members
- Using Marketing Automation to remind members about events as well as remind organizers it's been awhile since they last hosted an event or simply for notifying users on waiting list that they got an spot at an event. There's a multitude of possibilities with this.

We would have loved to dig further into one of these more elaborate functionalities, but as the criteria of the judging is on a complete solution, we had to start with the creating and joining of events, and that's as far as we got.

## Video

[![Video Demostration](images/yt.png?raw=true)](https://youtu.be/esH0gBIHsgo)
