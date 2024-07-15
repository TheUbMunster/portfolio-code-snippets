# "Website" - Solo Project - Ongoing - HTML/CSS/Javascript

This website is probably the longest in-development project of mine. It's features reflect what I've learned over the years, in both hard and soft skills.

The original version of this portfolio webpage had a simple CSS styling for my code snippets that was pulled from a technichal writing class where I wrote some
microsoft-esq documentation on a programming language I'm desining. It was very simple, but then it evolved to support multiple "tabs", to take up less space.
Eventually I automated the process of reading in text files containing my code snippets on the webserver, and even used a code [highligher library](https://highlightjs.org/)
for cool syntax highlighting.

This was good and all, but as I continued to add new and old projects to this portfolio page, the idea of keeping source synchronized with up-to-date versions
became tedious, and eventually I tried to automate this process, and became aware of Github's Rest API. Although it had some growing pain, I'm pleased that content
is only fetched when a user attempts to view it (i.e., code snippets are loaded & highlighted JIT). Line numbering & the background gradient are implemented post-highlighting.

Of course, the purpose of this webpage is to display my knowledge. I could have used a cookie-cutter solution with a pleasing modern appearance (and I'm all for working smart,
and in most situations, tools like squarespace are the way to go), but I feel that would defeat the purpose of a portfolio specifically for a computer scientist.

If you're curious how it works, always remember...

<p align="center" style="max-height: 50vh;">
    <img src="https://raw.githubusercontent.com/TheUbMunster/portfolio-code-snippets/main/Website/internet%20tshirt.jpg"/>
</p>