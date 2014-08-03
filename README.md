Penguin
=======

Penguin, send Everybody Edits commands in plain English

Please note that this repository has been heavily inspired by https://github.com/Seist/NaturalLanguageProcessor


Penguin processes messages in six stages.

1. The message is recieved by Penguin (through PlayerIO). The uninteresting words are removed.
2. The message is then split apart into seperate commands (if the user sent seperate commands in the same phrase)
3. The block descriptions are then tokenized into block ids
4. The message then is then filtered more. Penguin translates this message into Amber.
5. Amber gets the message and performs optimizations like message duplication reduction and message replace and remove combining. This reduces requests to the server.
6. Amber then converts the resulting message into C# code (or something equivalent). This code can then be processed by your program to send actions to the world.
