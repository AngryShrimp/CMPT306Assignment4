Keenan Johnstone - Assignment 4 - CMPT306
11119412 - kbj182

README
------
To use, hit the play button (ctrl-p) then select the player game object from the Heirarchy. Under the inspector there are 3 options to mess around with:
	Body Mass: Scales the muscular-ness of the UMA (Only works on Male, currently, but was just for fun)
	IsMale: Toggles the Body Type
	Happy: A slider which adjust the happines of the UMA, because we should be able to control thier emotions

Algorithm Description
---------------------
All of the run time updates are done through the Update() tick. 

For Happiness I used the expressionplayer for provided in the UMA to easily control the facial structure on any UMA, which allows for it to be applied to any UMA. On each tick of update we check to see if the value for the Happiness has changed and if they have we adjust the appropriate value on the expression player.

For the Body Type swapping I use a boolean toggle. Weh triggered a previously created UMA is loaded (Male or Female, under resources).

Because of how the Facial expressions can be transferred, if the male is grumpy, then you swap to the female, they will also be grumpy.


Description of K_Looks_Good
---------------------------
The K_Looks_Good Values provided in the UMA were used for this assignment meaning that all of the values used were constrained to what (mostly) normal human features would allow.
