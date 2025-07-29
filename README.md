# 🪙 Bounce To Earn on Etherlink

**Bounce To Earn** is a simple yet addictive blockchain game where players jump to avoid obstacles and accumulate points. The game is built with **Unity Engine**, integrated with Web3 via **Thirdweb Unity SDK**, and runs directly on the **Etherlink Testnet**.

> 🎯 Play now to earn – Testnet tokens are waiting for you to claim!

---

## 🚀 Key Features

- 🌐 **Web3 Integration via Thirdweb Unity SDK**

  - Connect wallet and interact with contracts on Etherlink Testnet

- 🛡️ **Smart Contract Token Gate NFT**

  - Players **must claim the Token Gate NFT before playing**
  - Without this NFT, access to the game is restricted

- 👑 **VIP NFT**

  - Doubles the player's score
  - Unlocks **special characters** in the game

- 💎 **GEM Token**

  - Used to spin the Fortune Wheel
  - Can receive **Revival Tokens**

- 🌀 **Fortune Spin Wheel**

  - "Burn to spin" mechanism: use GEM tokens to spin
  - Win gameplay-boosting rewards

- 🎮 **Fast-paced Gameplay**
  - Tap to bounce and avoid obstacles
  - **Accumulate points to exchange for tokens**

---

## 🌍 Try the Game

👉 [Play the WebGL version on Itch.io](https://thkien85.itch.io/etherlink-gamefi-bounce-bounce)

---

## 🛠 How to Use the Source Code

### Requirements:

- Unity 2022 or higher
- **Thirdweb Unity SDK** installed

### Instructions:

1. Clone the project:

   ```bash
   git clone https://github.com/KelvinTrinhSG/etherlink-bounce-to-earn-gamefi.git
   cd Etherlink-bounce-to-earn-gamefi
   ```

2. Open the project in Unity using your preferred method.

---

## 🧩 Technologies Used

- 🎮 Unity Engine
- 🔗 Thirdweb Unity SDK
- 🌐 Etherlink Blockchain (Testnet)
- 🧠 Smart Contracts: Token Gate NFT, VIP NFT
- 💰 Tokens: Testnet token, GEM
- 🔁 Revival Token managed via PlayerPrefs

---

## 🔐 Token-Gated Access

### The Token Gate NFT

- **Type:** OpenEditionERC721
- **Symbol:** GATE
- **Purpose:** Grants access to the game
- **Description:** A digital pass that unlocks core features and access
- **Etherlink Contract:** `0x6B7FFDe4650268B98A0DE2438906881Bb2f8450F`
- [View Contract](https://thirdweb.com/team/kelvincod/0e7eed2e2e708515a11d78eaedf37f02/contract/128123/0x6B7FFDe4650268B98A0DE2438906881Bb2f8450F)

### The VIP NFT

- **Type:** OpenEditionERC721
- **Symbol:** VIP
- **Purpose:** Doubles score, unlocks special character
- **Description:** Premium access with exclusive benefits
- **Etherlink Contract:** `0x5b889D548b142aF5D68e37A9489A77656C64b117`
- [View Contract](https://thirdweb.com/team/kelvincod/0e7eed2e2e708515a11d78eaedf37f02/contract/128123/0x5b889D548b142aF5D68e37A9489A77656C64b117)
- **Cost:** 2 XTZ

---

## 💎 The GEM Token

- **Type:** ERC20 Token Drop
- **Symbol:** GEM
- **Purpose:** Used to spin the Fortune Wheel
- **Description:** Earned or purchased; rewards player actions
- **Etherlink Contract:** `0x3Ba58FE809406ac88E20F7D937506bB8Eb39B159`
- [View Contract](https://thirdweb.com/team/kelvincod/0e7eed2e2e708515a11d78eaedf37f02/contract/128123/0x3Ba58FE809406ac88E20F7D937506bB8Eb39B159)
- **Cost:** 1 XTZ

---

## 🔄 Gameplay Flow

1. Player connects wallet
2. Claims Token Gate NFT to enter
3. If VIP NFT is owned, score is doubled and a bonus character is unlocked
4. Player earns or purchases GEM tokens
5. GEM tokens are burned to spin the Fortune Wheel
6. The Fortune Wheel rewards Revival Tokens
7. Players accumulate scores, which can be submitted onchain to claim XTZ (future support)

---

## ⚙ Scalability

- Client-side gameplay logic ensures smooth performance
- Only key actions (claim, burn, earn) are onchain
- Modular NFT/token utility for future upgrades without full redeploy
- Etherlink's high throughput and low fees enable seamless scaling

---

## 🤝 Contributing

We welcome all contributions to improve the game:

- Fork the project
- Create a new branch `feature/your-feature`
- Submit a Pull Request with clear descriptions

---

## 📬 Contact

- 💬 Telegram: [@KelvinTGameDev](https://t.me/KelvinTGameDev)
- 🐦 Twitter: [@CoinInves2024](https://x.com/CoinInves2024)

---

## 💚 For the Etherlink Community

We are officially submitting this game for the **Etherlink Hackathon 2025**. This is our most dedicated product to support the Etherlink ecosystem.

> **Claim your NFT – jump – and earn on Etherlink Testnet!**
