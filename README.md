This is a simple custom language interpreter built for my thesis project.

It has three main components:
- a lexer(tokenizer), which performs a simple tokenization of the source code
- a parser, which tries to construct an abstract syntax tree, using the recursive-descent method
- an interpreting context, which is a state holder for the execution of the syntax tree


This project is only a part of the entire thesis project, which is a team effort. The use case involves feeding various objects into the language's context, so the API is kind of weird. Don't mind that.
