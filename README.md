Penguin
=======

Penguin, send Everybody Edits commands in plain English

Please note that this repository has been heavily inspired by https://github.com/Seist/NaturalLanguageProcessor


_Penguin was created to support multiple commands per message._ This is one of the main reasons why Penguin was created.

Penguin processes messages in six stages.

1. The message is recieved by Penguin (through PlayerIO). The uninteresting words are removed.
2. The message is then split apart into seperate commands (if the user sent seperate commands in the same phrase)
3. The block descriptions are then tokenized into block ids
4. Words that are residules from the tokenizer are then removed. Penguin translates this message into Amber.
5. Amber gets the message and performs optimizations on the messages to try to combine them together if possible. Optimizations can usually decrease the overall operation time.
6. Amber then converts the resulting message into C# code (or something equivalent). This code can then be processed by your program to send actions to the world.

Penguin can make very large optimizations with complex queries if you allow Penguin read-only access to the map. If you do not allow Penguin the map, it can only perform superficial optimizations.
