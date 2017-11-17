# NBC

The worst bytecode you will ever see

In its current state, the project is simply just an assembler for it, but will soon contain both the assembler and an interpreter which may or may not use Unity. I do not know yet.

Documentation:
Architecture:

- RAM:

  - 16 pages of RAM each 256 bytes
    - 16*256=4096 (4KB)
  - 17th page is an inter-process page
  - In the program header, up to 256 bytes of RAM can be reserved and set to read-only for program data. These are placed after page 17.
    - Example:
    ```smilebasic
    prg[0]=32;
    prg[1-31]=Address of page in the Program
    ```
  - RAM is completely seprate from program memory.
    - This means that you cannot edit your program while it is running
  - How to access a certain page:
    - Use the chp opcode:
    ```asm
    chp %5
    ```
- ASM structures:
  - Case is unimportant, except for labels
  - All files must contain at least these three directives:
  ```.nbcasm {version}
  .osbyte {whatever value you want to send to the OS}
  .targetarch {your target architecture ID}
  ```
  - Labels are denoted as such
    ```asm
    :label
    ```
  - Value types:
    - $ - Address
    - \# - Indirect address
    - % - Literal
    - l/L - 16-bit
    - s/S - 8-bit
    - : - Label
    - Example:
    ```asm
    mov $l6 #s5
    ```
      - Moves the value in address 6 (which is a 16-bit value) into addres 5 (Which is an 8-bit value. Each conversion from 16-bit to 8-bit gets the first 8 bits cut off.

- Command structures:
  - Opcode:
    - Word 1: Command
    - Word 2: Flags
      - Flags (Gives flags for each argument):
        - Bit1: Present
        - Bit2: 8/16-bit
        - Bit3: Const/Address
        - Bit4: Direct/Indirect
          - Bit 4 requires bit 3 to be set.
    - Words 3/Bytes 5 on are arguments to a max of 4 arguments

- Program Structure
  - Header:
    - Byte 1: Architecture ID
      0. Universal (Can handle any architecture)
      1. PC (Windows/Linux using the one provided in this repo)
      2. 3DS (SmileBASIC)
      * If you want to add more, just ask and I can put it in the list.
    - Byte 2: OS-Specific argument
    - Byte 3: Ammount of pages to reserve
    - Byte 4-x Addresses of the pages that are reserved (Multiple of 256)

- Commands:
  - `NOP`
    - Opcode 00h
    - Does nothing. Usually used for padding
  - `HLT`
    - Opcode 01h
    - Ends the program
  - `CHP page`
    - Opcode 10h
    - Changes the current page of RAM
  - `MOV src dest`
    - Opcode 11h
    - Moves the value of src to the address of dest
      - dest must be of type address
  - `MOL src dest page`
    - Opcode 12h
    - Moves the value of src to the address of dest with the page specified
      - dest must be of type address
  - `INC addr`
    - Opcode 20h
    - Increments the value in the address specified
      - addr must be of type address
    - Sets carry flag if value is too large
  - `DEC addr`
    - Opcode 21h
    - Decrements the value in the address specified
      - addr must be of type address
    - Sets carry flag if value is too small
  - `ADD op1 op2 dest`
    - Opcode 22h
    - Adds two operands and stores the result in dest
      - dest must be of type address
    - Sets carry flag if value is too large
  - `SUB op1 op2 dest`
    - Opcode 23h
    - Subtracts two operands and stores the result in dest
      - dest must be of type address
    - Sets carry flag if value is too small
  - `MUL op1 op2 dest`
        - Opcode 24h
        - Multiplies two operands and stores the result in dest
            - dest must be of type address
        - Sets carry flag if value is too large
  - `DIV op1 op2 dest`
    - Opcode 25h
    - Divides two operands and stores the result in dest
      - dest must be of type address
    - Only returns integer values
  - `DIR op1 op2 dest`
    - Opcode 26h
    - Divides two operands and stores the remainder in dest
      - dest must be of type address
  - `JMP addr`
    - Opcode 30h
    - Jumps to the address specified in addr
      - addr must be of type address
  - `JMC addr`
    - Opcode 31h
    - Jumps to the address specified in addr if carry flag is set
      - addr must be of type address
  - `JME addr`
    - Opcode 32h
    - Jumps to the address specified in addr if equal flag is set
      - addr must be of type address
  - `JMG addr`
    - Opcode 33h
    - Jumps to the address specified in addr if greater than flag is set
      - addr must be of type address
  - `JML addr`
    - Opcode 34h
    - Jumps to the address specified in addr if less than flag is set
      - addr must be of type address
  - `JNC addr`
    - Opcode 35h
    - Jumps to the address specified in addr if carry flag is not set
      - addr must be of type address
  - `JNE addr`
    - Opcode 36h
    - Jumps to the address specified in addr if equal flag is not set
      - addr must be of type address
  - `JNG addr`
    - Opcode 37h
    - Jumps to the address specified in addr if greater than flag is not set
      - addr must be of type address
  - `JNL addr`
    - Opcode 38h
    - Jumps to the address specified in addr if less than flag is not set
      - addr must be of type address
  - `JSR addr`
    - Opcode 39h
    - Jumps to the address specified, storing the calling address in a stack
      - addr must be of type address
  - `RET`
    - Opcode 3Ah
    - Returns to the last address pushed to the stack + 1
  - `CMP op1 op2`
    - Opcode 40h
    - Sets flags accordingly when comparing op1 and op2
    - Equal, greater than, and less than flags may be changed
  - `OUT port value`
    - Opcode 50h
    - Outputs to the port specified the value specified
      - Only takes 16-bit values
  - `IN port dest`
    - Opcode 51h
    - Takes input from the port specified and stores the input in dest
      - dest must be of type address
      - Only takes 16-bit values
  - `PSH value`
    - Opcode 60h
    - Pushes a value to the stack
  - `POP dest`
    - Opcode 61h
    - Pops a value from the stack to the destination dest
      - addr must be of type address

- Ports info:
  - Fully standard (must be present)
    - 00h - TTY character out
      - OUT - Outputs the character specified in TTY form (Using ASCII)
      - IN - Always returns 0
    - 01h - Get character
      - OUT - Does nothing
      - IN - Gets currently pressed key in ASCII, returns 0 if nothing is pressed
    - 02h - Get current architecture
      - OUT - Does nothing
      - IN - Gets current architecture
  - Semi standard (Must be present, but can have small differences from OS to OS)
    - 10h - Cursor control
      - OUT:
        - First output should be x value
        - Second output should be y value
      - IN:
        - First input is current x value
        - Second input is current y value