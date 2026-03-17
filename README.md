# 🔐 encrytext

**Secure, Peer-to-Peer, End-to-End Encrypted Messaging in your Terminal.**

[Showcase video placeholder]()



`Encrytext` is a lightweight, terminal-based messaging application built for privacy and minimalism. By removing the central server, it establishes a direct link between users, ensuring that your data stays between you and your peer.

---

## 🚀 Key Features

* **P2P Architecture:** Direct connections between peers using **Socket.io** and **TCP**. No central servers storing your message logs.
* **End-to-End Encryption (E2EE):** Leverages the **Web Crypto API** for robust encryption.
* **Diffie-Hellman Key Exchange:** Securely generates a shared secret between users without ever sending the key itself over the wire.
* **Sleek TUI:** A high-performance Terminal User Interface built with **Blessed**, featuring real-time message rendering and a responsive input buffer.
* **Zero Logs:** Privacy is the default. No databases, no tracking, just code.

---

## 🛠 Tech Stack

| Component | Technology |
| :--- | :--- |
| **Language** | C# |
| **Networking** | Socket.io |
| **Security** | AES-GCM / Diffie-Hellman |
| **UI toolkit** | [Terminal.Gui](https://github.com/gui-cs/Terminal.Gui) |
