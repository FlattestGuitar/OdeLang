Ode is a simple programming language I built for my Engineer's Thesis.

It has three main components:
- the lexer, which performs a simple tokenization of the source code
- the parser, which tries to construct a valid abstract syntax tree, using the recursive-descent method
- the interpreting context, which is a state holder for the execution of the syntax tree


The language is only a part of the entire thesis project, which was a team effort. The other members of the team created a video game where the player can use Ode to program robots to solve puzzles.

Here are some language features:

Implicit type casting 
```python
print("2" + 2) # Output: 22
```

Function definitions
```python
fn add_two(num)
  return num + 2

print(add_two(5)) # Output: 7
```

Array manipulation
```python
arr = [1, 2, 3]
arr.append(4)
print(arr)     # Output: [1,2,3,4]
print(arr[1])  # Output: 2
```

Dictionaries
```python
dict = {
  "alice": 1,
  "bob": 2,
  "charlie": 3
}

for key in dict
  print(key + " has " + dict.get(key))

# Output:
# alice has 1
# bob has 2
# charlie has 3
```
