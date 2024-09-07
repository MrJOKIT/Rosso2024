Thankyou for purchasing Grid Builder 2.

First of all, I have finally (Jan 2023) set up a Discord  account, and for all future assets will include invites to the relevant channel. 
I will most likely be more responsive to support requests on this platform, plus I am sure other users may help from time to time.

Here is the link to join for Grid Builder 2 Discord - https://discord.gg/fBx4WYrzXh

Demo scenes are included in the scenes folder to help understand how the all of the components work. 
When using Grid Builder 2 for the first time the demo scenes are very helpful to know what to add. 
After getting to know the system, feel free to just ignore the Demo folder altogether when importing to save on file size. 

Current controls are set to Left mouse button to place, remove or select objects and right mouse button or escape to cancel.
Rotate is 'R'. 
You will find these Input controls in GridSelector.cs, RemoveMode.cs, ObjectRemover.cs and ObjectSelector.cs. 

Easiest way to start is to use a grid, and gridSelector prefab.

To set up quickly - 

1. Navigate to /Prefabs/WholeGrids, and drag in one of the premade grids and adjust the General settings such as height, width and cellSize. 
Best to keep the cellSize at 1 = 1 metre. However everything will adjust if you decide to change it. 
You can use as many grids as you want, 1 - 5 - 50!
If you want the grid to be above some existing terrain, move the grid on the Y axis slightly above. A value of 0.015 works good. 
Leave AutoCellBlock off for now. If you want to use it, please see the Setup.pdf supplied with the package. 

2. Navigate to /Prefabs and drag in a GridSelector, ObjectPlacer, and ObjectRemover. 
You should only have one of each of these. There are lots of other prefabs to add, which you can lots more info on in the docs section of the website.
https://golemitegames.com/index.php/docs/
You can also see what components/prefabs require what here - https://golemitegames.com/index.php/docs/component-descriptions-requirements/

3. Back in /Prefabs, drag in a SelectObjectBtn and RemoveModeBtn. 
This will create a canvas so you can move the buttons somewhere convenient. 
You will need an already set up prefab to place on the SelectObject button prefab slot. Please note there is no scaling done to prefabs,
so the models will have to match the grid size. You can find how to set that up here -
https://golemitegames.com/index.php/docs/setting-up-the-selectobject-button/

4. Thats it! Hit play and you should be able to move, click on an object and place it down on the grid. 
You can also hit the removeMode button to highlight and delete objects. 

5. After this there is quite a bit to play around with, particularly the AutoCellBlock feature.
Its super simple and quick to set up once you know how it works. See here - 
https://golemitegames.com/index.php/docs/setup-auto-cell-blocking-2/

6. Happy building! If you are happy with the asset please leave me a review if you have the time, it really helps!
If there is anything you are not happy with, please contact me on the email below, so I can try to resolve the issue. 

Full documentation is available at https://golemitegames.com/index.php/docs/

If you have any questions or are having trouble setting up please dont hesitate to contact me at support@golemitegames.com






