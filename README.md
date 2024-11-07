### Vowel
This is a small dynamically typed language written in C#

##
Comparing strings and numbers is allowed
things like 

	"hey hey" > 4 // this would yield true

The interpreter simply gets the length of the left operand (string) and executes the 
instruction
	
##
The `?:` operator will work even without the else part, though it'll return 'nil' 

For example
	
	var c = (3 > 4) ? "c string"; 
	wandika c; // this will yield 'nil'

The interpreter will issue a warning, indicating to the user about what's about to happen