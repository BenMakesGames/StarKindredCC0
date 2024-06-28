"Utility" is a really bad name for, like, anything. It's too generic; it doesn't mean anything; no one who looks at something named "Utility" can possibly guess what lies within. Never name something "Utility" >_>

The classes in this folder are mostly used by just one feature. For example, "TownHelpers" has stuff related to decorating your town, and information related to unlocking new spots in your town. Each of the methods in "TownHelpers" could be - SHOULD be - pulled close the endpoints that actually use them, and the "TownHelpers" class itself deleted.

A similar treatment can be given to the majority of classes in this folder. That'd be rad to do.