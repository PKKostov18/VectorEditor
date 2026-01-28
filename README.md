# VectorEditor 🎨

**VectorEditor** is a Windows Forms application developed as a course project for **CSCB579 - Programming with Microsoft Visual C# .NET**.

The application allows users to create, manipulate, and animate geometric shapes (Rectangles and Circles) on a canvas. It features dynamic animation, file serialization using JSON, and a bilingual user interface (English/Bulgarian).

---

## 🚀 Features

### 1. Vector Graphics & GDI+
* **Draw Shapes:** Create Rectangles and Circles using mouse drag-and-drop interactions.
* **Custom Colors:** Choose any color for the shapes using a Color Picker dialog.
* **Double Buffering:** Implemented to prevent screen flickering during rendering.

### 2. Animation & Dynamics 🎬
* **Physics Simulation:** Shapes automatically move and bounce off the window boundaries.
* **Control:** Start/Stop animation toggle to allow easier drawing.
* **Interaction:** Shapes continue to move in the background while the user draws new ones.

### 3. Serialization (Save/Load) 💾
* **JSON Format:** The project uses `System.Text.Json` for modern, secure data storage.
* **Polymorphic Serialization:** Correctly handles derived classes (`MyRectangle`, `MyCircle`) using `[JsonDerivedType]` attributes.
* **Persistency:** Saves position, size, color, and velocity of all objects.

### 4. Localization (Multilingual UI) 🌍
* **Languages:** Full support for **English** and **Bulgarian**.
* **Implementation:** Uses `.resx` Resource files and `ResourceManager` to switch strings dynamically at runtime without restarting the application.

---

## 🛠️ Tech Stack

* **Language:** C#
* **Framework:** .NET 6.0 / .NET 8.0 (Windows Forms)
* **IDE:** Microsoft Visual Studio 2022
* **Graphics:** GDI+ (System.Drawing)
* **Data:** System.Text.Json

---

## 🏗️ Architecture & OOP Concepts

The project demonstrates key Object-Oriented Programming principles:

* **Inheritance:** An abstract base class `Shape` defines common properties (`X`, `Y`, `Color`, `Dx`, `Dy`). Concrete classes `MyRectangle` and `MyCircle` inherit from it.
* **Polymorphism:** The `Draw(Graphics g)` method is abstract in the base class and overridden in derived classes to provide specific rendering logic.
* **Encapsulation:** All data fields are protected via Properties.
* **Clean Code:** Separation of logic into folders (`Models`, `Forms`).

---

## 📦 How to Run

1.  **Clone the repository:**
    ```bash
    git clone [https://github.com/YOUR_USERNAME/VectorEditor.git](https://github.com/YOUR_USERNAME/VectorEditor.git)
    ```
2.  **Open the project:**
    Open `VectorEditor.sln` with Visual Studio 2022.
3.  **Build & Run:**
    Press `F5` or click the **Start** button.

---

## 👤 Author

* **Name:** Plamen Kostov
* **Faculty Number:** F113851
* **Course:** CSCB579
* **University:** New Bulgarian University (NBU)

---
