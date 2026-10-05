InfoDex "Information Rolodex"

I am a Senior Software Developer/Engineer.  Like a lot of us I often have multiple Irons in the fire.  And, these Irons come in all 
shapes, sizes, and colors.  It is difficult sometimes to juggle of the particulars of what I am working on...especially when I put 
a task/project on pause and then return to it later.

InfoDex provides a simple method of storing contextual task/project details and an easy way to recall these particulars.  Data storage
is supplied by MS Access databases.  InfoDex (out of the box) provides a baseline template database (blank_database.mdb) of which other
contextual databases are created from (simple a copy from -> to operation via the InfoDex configuration page.  The baseline Access 
database provides the following Tables:  blank_table, DATA, ESCLUSION, and SECURITY (SECURITY is a placeholder for future encryption 
work).  Using Access databases may seem dated (and believe me, it is).  However Access provides a nice opportunity to import data from
other sources into a a context database, and then manage that data from with in InfoDex.  I worked for one Company who let go a Programmer
who was very protective of his email (and very sloppy).  I was asked to search through thousands of His email to find a tidbit of 
information (the proverbial "Searching for a Needle in a Haystack:).  I opted to import his email into an Access database using 
Data Transformation Services (DTS) is a legacy Microsoft database tool used to extract, transform, and load (ETL) data between 
different sources.  Everything worked out just fine.  Additionally, I installed InfoDex (a straight up copy deployment) onto our
Departmental Web server and made it available to everyone in my group.  This really reduced the number of times I had to pause what
I was working on in order to satisfy someone's ad-hoc data search and retrieval request (really nice).

Now, it is up to you how contexts (databases) are maintained.  When executed for the first time, there are no databases (contexts) 
configured.  What I like to do is access the InfoDex Configure menu item and create a New database named "InfoDex" 
(the default Configuration page examines the Database folder and lists all Access Database files in a list on the left of the screen.
After I have created an InfoDex database, I return to the Search page, select "InfoDex" from the Database Dropdown list and enter
search criterial.  Initially these databases will be empty.  If your search details did not result in any hits, you may choose to 
create a new entry.

InfoDex eliminates all Forms of Be, Is Are Was Where Be Been Being, etc.  These terms are stored in the Access database EXCLUSION table.
The EXCLUSION table comes pre-configured with English Forms of Be.  This table structure is as follows: UID, WORD, FLAG, and ACTIVE. 

I mentioned above "It is up to you to decide how to setup databases".  I gravitated towards database names like Processes, PowerShell,
Ubuntu, VBScript_VBA, VisualStudio, WishList, Xamarin, etc..  Additionally, I would create specialized databases like Leveraged_UCMDB,
Kersted_Process_App, Semetric, etc..

Could I have written this better?  You bet.  However, I created the initial version years ago while working in Compaq Technical 
Support (a Tech Phone Jockey) fully written in VB.  Over time, InfoDex morphed into a Web Application, and further morphed with
various enhancements.

So, have fun...use this as you may.  Please share with others :o)
