//These "using" statements below are namespaces [Organized libraries of defined items in the code]. They are used to help shorten the code for the compiler so we dont have to write as much. For example, line 19 below has a variable named "e" of variable type Exception.
//The Exception type exists in the "System" namespace. By having the line "using System;" below, doing the "System." before the word Exception on line 19 is unecessary because the compiler assumes that the word
//Exception is under the System namespace because no other definition is present. If two seperate definitions are provided, distinction between the namespaces becomes necessary. Another example; the list of variable
//name "l" on line 20 is under the Generic namespace, which is under the Collections namespace, which is under the System namespace. (This fact of course implies that namespaces can be inside other namespaces).
using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

//This is the namespace that contains our program. If someone wanted, they could put the phrase "using Container;" at the top of their script to be able to have access to the definitions contained inside the "Container"
//namespace. (They would have the ability to create a variable that holds a reference to a "Program" object ("Program" of course being the class defined below). They would also have the ability to instansiate instances
// of one or more "Program"'s).
namespace Container
{
    //This is our class definition for "Program". Classes always exist on the HEAP; whereas Structs always exist on the STACK. The differences between these will be explained later.
    class Program
    {
        System.Exception e;
        System.Collections.Generic.List<object> l;
        //This is a Method. Also known as a Subroutine, or a Function. This Method has modifiers such as "public", and "static". The word void represents the return type, which will be explained later.
        public static void Main(string[] args)
        {
            
            //This program will output the words "Hello There!".
            Console.WriteLine("Hello There!");
            //and before it closes, it will await a keypress. If the line below wasn't there, it would print the words above then immediately close in the same frame. It can't reach the end of the application without
            //reaching the keypress.
            Console.ReadKey();
        }
    }
    //We will now discuss simple variable and method modifiers.
    /*
     * --====- These are by no means "official" definitions; these are in layman terms. -====--
     * public - (Valid on method, variable, and variable type definition declaration). - Makes the modified object able to be accessed by basically anyone that holds a reference to the method, variable, or definition.
     * private - (""). - Parents can see the children as long as they are defined in the parent; Hidden from almost everybody.
     * protected - (""). - Children can see parent variables etc.; hidden from everyone else.
     * static - (""). - This modifier means theres only one of the modified item in existense. (It cannot be instansiated).
     * abstract - (""). - This modifier means that the modified item cannot be instansiated directly, but it can be inherited from a child instanse. (A HEAP object can be inherited from by default).
     * virtual - (""). - This modifier means that the modified item can be instansiated or inherited.
     * sealed - (""). This specifies that the modified object cannot be inherited from.
     * *FOR MORE OFFICIAL DOCUMENTATION; OBVIOUSLY LOOK UP THE OFFICIAL DOCUMENTATION. (They'll probably explain it better too).*
     */
    //Here we have a new class definition declaration for a class named "Dog".
    public class Dog
    {
        //Inside the class we will have two functions; a private one and a public one.
        public void Bark()
        {
            OpenAndCloseMouth();
        }
        private void OpenAndCloseMouth()
        {
            //Do stuff.
        }
    }
    //Here we have a dummy class to do stuff with our "Dog" class above. it's a bad idea to do recursive programming. (Creating instanses of stuff inside of itself; this will go on forever).
    class Dummy
    {
        //Here we declare a variable to hold a REFERENCE to an instanse of a "Dog" class, but it's currently not holding one because we make the variable that has the ability to hold a "Dog" instance, but one is never 
        //created.
        Dog dog; //If we wanted to assign it a brand new instanse of a "Dog" class inline, we would add " = new Dog();" in between the g and the semicolon.
        //By the way, in C#, if an access modifier is not specified, it is assumed to be "private". (Therefore our "dog" variable is private).
        //This function can never be called since there's nothing inside the "Dummy" definition that calls it, and since the Dummy definition is private, nothing can call it from outside. We're making it to house stuff
        //for the sake of showing how it works.
        void MakeNewDogAndBark() // <-- Is a private function since no access modifier is specified.
        {
            dog = new Dog(); // <-- Assignes a brand new dog instance to our "dog" variable.
            dog.Bark(); // <-- Here we are making our dog instance bark.
            //dog.OpenAndCloseMouth(); <-- this throws an error saying "dog.OpenAndCloseMouth is inaccessible due to its protection level". this is because our "OpenAndCloseMouth" function on line 51 is private. It 
            //can't be called from outside.
        }
    }
    //Here we are making a new class; "Mastiff". Our Mastiff DERIVES from a dog, since it is a dog. We want some features that mastiffs have, but other dogs don't. Our Mastiff has all the features that Dog does, like 
    //Bark(), but we're making some extra stuff. A DERIVED script can be cast (this means transformed, or "forcefully viewed as") its parent object. (You can treat a Mastiff as a instanse of Dog, but not the other way)
    //around, since some dogs are mastiffs, but not all dogs are.
    class Mastiff : Dog
    {
        void BeIntimidating()
        {
            Bark();
            Bark();
        }
    }
    struct DummyStruct
    {
        int numberJustBecause;
        bool boolBecauseWhyNot;
    }
    class DummyClassTwo
    {
        DummyStruct dsOne = new DummyStruct();
        DummyStruct dsTwo = new DummyStruct();
        Dog dOne = new Dog();
        Dog dTwo = new Dog();
        void Stuff()
        {
            dTwo = dOne;
            //then if we change a value is dOne, it changes in dTwo as well. Both variables are referring to the EXACT SAME OBJECT; EXACT SAME MEMORY ADDRESS.
            dsTwo = dsOne;
            //In the case directly above, we are simply creating a clone of dsOne and assigning it to dsTwo. If we change a value in dsOne after the assignment, it does not change in dsTwo.
        }
    }
    //Now we will be looking deeper into how we utilize functions. Functions can do really cool calculations and operations for us in a neat wrapped up box. However, most of the time, if not all of the time, we want to
    //actually input data into the function for it to use. These are called function parameter0s.
    class ParametersExample
    {
        private void ParameterFunction(int parameterOne, bool parameterTwo, char parameterThree) // <-- As you can see, this function has three parameters. the syntax is --> FunctionName([Variable Type] [Variable Name])
        {
            //If you want more than one parameter, seperate them by commas.
            //Now we have access to the "parameterOne", two, and three variables inside of the function. the values of these variables are determined by whoever calls the function.
            char c = parameterThree;
            bool b = parameterTwo;
            int i = parameterOne;
        }
        private void CallExample()
        {
            //Here we're going to call our parameterFunction above. we put each value into its respective spot. (Ordered properly; same as above. int, bool, then char).
            ParameterFunction(5, false, 'f');
        }
        //Another thing about functions and how cool they are, we can actually get data back from them. This is called returning a value.
        //Notice how all of our functions so far have had the word "void" preceeding them. This means that they do not have a return type. (void meaning, none; nada, etc.). If we want to return a value of a certain type,
        //all we have to do is replace that "void" with a type, such as "int" or "float" or "char"
        private int OurCustomAddFunction(int firstNum, int secondNum, int thirdNum)
        {
            int result = firstNum + secondNum + thirdNum;
            //Now, to return our value, we have to use a new keyword. "return". we type "return" followed by a literal typed value or a variable of the return type. (See line below).
            return result;
        }
        //We will make a dummy function below to show how to get the value out of the function.
        private void DummyFunction()
        {
            int resultOfAddedNumbers = 0;
            resultOfAddedNumbers = OurCustomAddFunction(3, 5, 2);
            resultOfAddedNumbers = OurCustomAddFunction(OurCustomAddFunction(0, 1, 2), 4, 3);
            //resultOfAddedNumbers now equals 10!
            //Since "OurCustomAddFunction" returns an int, we literally can treat the whole "OurCustomAddFunction(3, 5, 2)" statement as an int, since that's what it returns. Setting something equal to a function that
            //returns a value will call that function inline and return that value. Functions can also return more than one value using the "out" keyword, but we'll talk about that later.
        }
    }
    //Now we're going to talk about Constructors. Wouldn't it be nice if whenever we made a "new Thing()" we could automatically specify certain things to be done to it upon it's creation? This is exactly what a
    //constructor is. In this example, we're going to show you how to make a constructor for our struct below.
    struct CoolThing // <-- By the way, structs are stored on the stack, classes are stored on the heap.
    {
        string coolnessValue; //If we were to initalize "coolnessValue" here, (meaning doing a new string()) it would throw an error, saying something along the lines of "you cannot initialize variables inside the body
        //of a struct." (This means, you need to initialize the values of your struct variables inside your constructor).
        //The name of the struct function is the same name as the struct/class name. (See below).
        public CoolThing(string howCoolAmI) // <-- "CoolThing" is simultainously the function name AND the return type. Since whenever we're making a "new CoolThing()", we are making a new instance of a CoolThing, (So we want the 
        {
            //Constructor to return our newly constructed CoolThing. Explicit struct constructors must have at least one parameter, whereas class constructors do not need at least one parameter.
            coolnessValue = howCoolAmI;
            //Now, whenever someone does a "new CoolThing([insert string here]);" it will call this function.
        }
    }
    //Now we're going to talk about explicit casting. Lets say you were doing some operations on a float, (A float is like an int, except it can hold decimals). And you got to a point where you needed it to be an int
    //(you don't care if it gets rounded). We're going to show you how to do that below.
    class CastingExample
    {                
        private void Example()
        {            
            float resultOfMathyStuff = MathyStuff();
            int result = 0;
            //To set our result equal to resultOfMathyStuff, we need to first cast it as an int, we do that by typing "([variable type you want to cast your value to])" directly before the variable you're casting.
            result = (int)resultOfMathyStuff; // <-- resultOfMathyStuff is being cast as an int because of the "(int)" before the variable.
        }
        private float MathyStuff()
        {
            float f = 5.5f; // <-- By the way, when setting a value to a float, you need to include an "f" at the end to specify it's a float value. (Otherwise the compiler would think you're doing "5[dot operator]5").
            //Do mathy stuff with f.
            return f;
        }
    }
}
//Here we go! you've learned a lot so far, lets see what you can put to use.
//CHALLENGE ONE:
/*
 * 1. Make a new project.
 * 2. Make a class that derives from another class, where the parent class has a public function that can be called from an instance of the child from an outside source. Some functions must take parameters/return values.
 * 3. Make a custom struct that holds 3 or more primitive type variables. Make a variable of this struct inside the child class and instantiate it; have the ability to change some values. (Try using a constructor).
 * 4. Call these functions and change said values inside of your Main() function inside your primary class. Use an explicit cast.
 * 5. Scroll Down to Line 200 for a possible solution.
 */



















//Begin Challenge One solution.
namespace ChallengeOne
{
    class Primary
    {
        public static void TheNameOfThisFunctionShouldSimplyBeMain(string[] args)
        {
            Ant ant = new Ant("Alfred the ant");
            float segmentCount = 3.1f;
            ant.myBody = new Body((int)segmentCount, true, 'A');
            ant.myBody.bodySegmentCount = 1;
            ant.myBody.firstLetterOfBugName = 'B';
            ant.myBody.isAlive = false;
            ant.myName = "Bobby the beetle";
            //Ant is now a dead beetle.
            string s = ant.DoBugThings();
        }
    }
    struct Body
    {
        public Body(int bsCount, bool alive, char flobn)
        {
            bodySegmentCount = bsCount;
            isAlive = alive;
            firstLetterOfBugName = flobn;
        }
        public int bodySegmentCount;
        public bool isAlive;
        public char firstLetterOfBugName;
    }
    class Bug
    {
        public string DoBugThings()
        {
            //Do stuff.
            return "I did a bug thing";
        }
    }
    class Ant : Bug
    {
        public Ant(string nameToAssign)
        {
            myName = nameToAssign;
        }
        public Body myBody;
        public string myName;
    }
}
//End Challenge One solution.