# Neural Network Letter Recognition Project

## Overview

This project implements a simple neural network using C# and the Accord.NET library to recognize letters based on input from a DataGridView interface. The project allows users to define letter shapes by coloring cells in a grid and trains the network to recognize five predefined letters: A, B, C, D, and E. The user can train the network, test it by drawing letters in the grid, and view the training progress and output results.

## Features

- **Neural Network Structure**:
  - Two layers (hidden and output) with customizable input and hidden layer sizes.
  - Randomly initialized weights for both layers.
  
- **Training Data**:
  - A predefined dataset of 5 letters (A, B, C, D, and E) represented as binary matrices.
  - Backpropagation algorithm to adjust weights during training.
  
- **Data Input**:
  - The user can draw letters in a 7x5 grid where each cell can be toggled between black (1) and white (0).

- **Feedforward Process**:
  - The network uses sigmoid activation for hidden and output layers.

- **Training and Testing**:
  - The network can be trained to minimize error using a backpropagation algorithm.
  - The user can test the network by drawing a letter in the grid, and the network will try to guess the letter.

- **Error Rate**:
  - After each iteration, the total error is displayed to help track the training process.

- **Saving and Loading Weights**:
  - Weights can be saved to a file after training.
  - Pre-trained weights can be loaded for testing without retraining the network.

- **Grid Customization**:
  - The user can toggle grid lines on or off for a cleaner look.

- **Interactive Interface**:
  - Users can clear the grid, reset the network, save and load weights, and modify the learning rate via the interface.

## How It Works

### 1. Neural Network Initialization
- The neural network consists of two layers:
  - **Input Layer**: 35 nodes (7 rows x 5 columns grid) representing the drawn letter.
  - **Hidden Layer**: 10 nodes (adjustable) to capture complex patterns.
  - **Output Layer**: 5 nodes representing the letters A to E.
- Random weights are assigned to both layers using the `InitializeWeights()` function.

### 2. Training
- The network uses the backpropagation algorithm to adjust weights:
  - **Feedforward**: The input matrix is flattened, passed through the hidden layer, and then to the output layer.
  - **Error Calculation**: The difference between the expected and actual output is used to compute the error.
  - **Backpropagation**: Gradients are calculated for both the hidden and output layers, and weights are updated accordingly.
- The process repeats for a fixed number of iterations, and the total error is displayed after each iteration.

### 3. Testing
- After training, the user can draw a letter in the grid and press the "Define" button.
- The drawn letter is converted to a binary input, passed through the network, and the output layer provides a guess for the letter.

### 4. Saving and Loading Weights
- The network’s weights can be saved to a text file, allowing users to avoid retraining the network in future sessions.
- Weights can also be loaded from a file to resume testing.

## Interface

![YSA](https://github.com/user-attachments/assets/d5a4640b-3142-425c-9450-fae0a18c7f61)

- **DataGridView**: The main area where users draw letters by clicking on the cells to toggle between black and white.
- **Labels**: Displays the outputs for each letter (A, B, C, D, E) after testing.
- **Buttons**:
  - **Train**: Starts the training process.
  - **Define**: Makes the network guess the letter drawn in the grid.
  - **Reset**: Resets the grid and neural network.
  - **Save**: Saves the current weights to a file.
  - **Load**: Loads previously saved weights.
  - **Toggle Grid Lines**: Toggles the visibility of grid lines for a cleaner interface.
- **NumericUpDown**: Allows the user to set the error threshold for training.

## Getting Started

1. **Requirements**:
   - .NET Framework
   - Accord.NET Library

2. **Running the Program**:
   - Open the project in Visual Studio.
   - Build and run the project.
   - Use the grid to draw letters and train the network using the predefined dataset.
   - Test the trained network by drawing letters in the grid.

3. **Training the Network**:
   - Press the "Train" button to start the training process.
   - The error rate will be displayed after each iteration.
   - Once the error is below the threshold, training will complete automatically.

4. **Testing the Network**:
   - After training, draw a letter in the grid and press "Define" to see the network’s guess.

5. **Saving/Loading Weights**:
   - After training, press "Save" to save the weights to a file.
   - Load weights from a previous session by pressing "Load."


## Conclusion

This project demonstrates a simple implementation of a neural network for letter recognition. It provides a hands-on way to understand the basics of feedforward neural networks, backpropagation, and how neural networks can be trained to recognize patterns in visual data.
