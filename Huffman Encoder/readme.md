# "[Huffman Encoder](https://github.com/TheUbMunster/portfolio-code-snippets/tree/main/Huffman%20Encoder)" - Solo project - ~August 2020 - C#
This was written by me during the Pokemon project in CSIS 2500, and was intended to be a method of heavily compressing and decompressing the pokemon JSON for the
sake of our 5MD database data cap. However at a later point we changed the design where the JSON was no longer sent to the database. As a result, this code wasn't
used in this project but has been tested and seems to work great. The "Grand" class contains utility methods, and LZCompressor is an unfinished implementation
of the LZ compression method. (Intent was to run the JSON through the LZ compressor then through the Huffman compressor to see if this gave better results than
the Huffman compressor alone). I had a caching system, in hindsight I don't know why I didn't just make the HuffmanCoder objects immutable.