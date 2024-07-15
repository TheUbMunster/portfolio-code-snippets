# "[Online CTR](https://github.com/CTR-tools/CTR-ModSDK)" - Team Project - June 2024 - C/C++ - [Mod Page](https://www.online-ctr.com/)

With some modding experience under my belt, a friend of mine notified me of the existence of a team of modders working on my favorite game of all time, Crash Team Racing.
A PS1 classic with great modding potential (some thunder of which was taken by the recentish remaster of the game), was something that I felt like contributing to.

My recent contribution to the repo consisted mainly of a PINE implementation. To paint a picture, the way that online multiplayer was implemented as a mod, was to use a certain
emulator, called [duckstation](https://github.com/TheUbMunster/duckstation). Duckstation was special in that the memory that the PS1 game resided in during emulation was public,
and could be accessed from other processes. The client/networking code isn't actually present in the PS1 code, but networking features are instead implemented via a "Networking client"
that runs alongside duckstation, and accesses/manipulates the memory of the game itself in order to deliver online multiplayer and further mod functionality. Although this
worked fairly well, synchronization was technechally not guaranteed in any way. In addition to that, the ability to access memory in this way was not really intended, and currently
only works on windows due to what I assume is a a [platform parity mistake](https://github.com/stenzek/duckstation/pull/3219).

My contribution was to implement memory access through an API called PINE, originally "invented" by the popular emulation framework [MAME](https://www.mamedev.org/). Stenzek (the main duckstation
developer) had implemented a PINE API implementation with the intent for communication via localhost/TCP. Unsuprisingly, using syscalls & the like to allow for direct memory access
was much faster than using the TCP stack, but it had to be done. My PINE implementation for the networking client allows for concurrent reads and writes, so long as the developer
is fully aware and codes accounting for data dependencies. With a nice C++ template class to wrap memory access, I wrote a nearly transparent system for interacting with memory.
During this process, I actually found a couple bugs in the emulator itself.

The nice thing about my system is, is that after we learn more about a [cool developer tool](https://www.orionsoft.games/retroshop/ps1usb.htm) allowing for memory access & manipulation
on an *actual PS1*, I could implement another backend implementation for my deferred memory model, allowing for easy development for both an actual PS1, and for emulators.

Although this is an amazing team project, and I did have a lot of help testing PINE implementation (Thanks Redhot!), PINE implementation was primarily written by me. 
[See this PR for my contributions.](https://github.com/CTR-tools/CTR-ModSDK/pull/152)