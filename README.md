<!--
*** Thanks for checking out this README Template. If you have a suggestion that would
*** make this better, please fork the repo and create a pull request or simply open
*** an issue with the tag "enhancement".
*** Thanks again! Now go create something AMAZING! :D
-->





<!-- PROJECT SHIELDS -->
<!--
*** I'm using markdown "reference style" links for readability.
*** Reference links are enclosed in brackets [ ] instead of parentheses ( ).
*** See the bottom of this document for the declaration of the reference variables
*** for contributors-url, forks-url, etc. This is an optional, concise syntax you may use.
*** https://www.markdownguide.org/basic-syntax/#reference-style-links
-->
[![Contributors][contributors-shield]][contributors-url]
[![Forks][forks-shield]][forks-url]
[![Stargazers][stars-shield]][stars-url]
[![Issues][issues-shield]][issues-url]
[![MIT License][license-shield]][license-url]
[![LinkedIn][linkedin-shield]][linkedin-url]



<!-- PROJECT LOGO -->
<br />
<p align="center">

  <h3 align="center">UnityCurve</h3>

  <p align="center">
    A simple system that create a generic implementation for an ADSR envelope; modulating a parameter over time in four distinct phases. This is based on Steve Swink's work, 'Game Feel'. The best part is that the user doesn't even need to do any scripting to get this system to work. Simple members are exposed in the inspector that provide full control over the parameter.  
    <br />
    <a href="https://github.com/DanteNardo/UnityCurve"><strong>Explore the docs »</strong></a>
    <br />
    <br />
    <a href="https://github.com/DanteNardo/UnityCurve">View Demo</a>
    ·
    <a href="https://github.com/DanteNardo/UnityCurve/issues">Report Bug</a>
    ·
    <a href="https://github.com/DanteNardo/UnityCurve/issues">Request Feature</a>
  </p>
  
  <a href="https://github.com/DanteNardo/UnityCurve">
    <img src="resources/ADSR Envelopes.png" alt="Logo" width="600">
  </a>
</p>



<!-- TABLE OF CONTENTS -->
## Table of Contents

* [About the Project](#about-the-project)
  * [Built With](#built-with)
* [Getting Started](#getting-started)
  * [Prerequisites](#prerequisites)
  * [Installation](#installation)
* [Usage](#usage)
* [Roadmap](#roadmap)
* [Contributing](#contributing)
* [License](#license)
* [Contact](#contact)
* [Acknowledgements](#acknowledgements)



<!-- ABOUT THE PROJECT -->
## What is UnityCurve?

[![Product Name Screen Shot][product-screenshot]](https://example.com)

UnityCurve provides a framework that grants users the ability to create multi-functional curves for integer, float, or double values only using the inspector. This was inspired by Steve Swink's ADSR envelope descriptions from 'Game Feel'. Here are some of its functions:

* Provides a **basic script that can modulate a variable's value with respect to time using four Excel Formulas**.
* Uses a **powerful C# calculation engine** to minimize finicky manual adjustments and leverage Excel calculations.
* Enables **callbacks whenever the formula for a parameter changes** via UnityEvents.
* Operates as an **extensible framework** that provides developers the ability to swap out core components.
* Includes a detailed **graphing interface** to visualize static and dynamic versions of your function curves.
* **Provides a wide range of useable ASDR implementation scripts**, including
  * ADSR_Axis
  * ADSR_Release


A list of resources that I find helpful are listed in the acknowledgements.

### Built With
While this project was primarily built with Unity3D, it is important to list the other resources.
* [Unity3D](https://unity.com/)
* [Visual Studio](https://visualstudio.microsoft.com/)
* [CalcEngine](https://github.com/Bernardo-Castilho/CalcEngine/)



<!-- GETTING STARTED -->
## Getting Started

To get a local copy up and running follow these simple example steps.

### Prerequisites

I plan on creating releases of an actual unitypackage once I hit a releaseable state, but until then you will need to add the files manually to a project. You will need the following in order to get the project running.
* Git client
* .NET Framework
* Unity3D 2020.1.1f1

### Installation

1. Clone the repo
```sh
git clone https://github.com/DanteNardo/UnityCurve.git
```



<!-- USAGE EXAMPLES -->
## Usage // TODO

Use this space to show useful examples of how a project can be used. Additional screenshots, code examples and demos work well in this space. You may also link to more resources.

_For more examples, please refer to the [Documentation](https://example.com)_



<!-- ROADMAP -->
## Roadmap

See the [open issues](https://github.com/DanteNardo/UnityCurve/issues) for a list of proposed features (and known issues).



<!-- CONTRIBUTING -->
## Contributing

Contributions are what make the open source community such an amazing place to be learn, inspire, and create. Any contributions you make are **greatly appreciated**.

1. Fork the Project
2. Create your Feature Branch (`git checkout -b feature/AmazingFeature`)
3. Commit your Changes (`git commit -m 'Add some AmazingFeature'`)
4. Push to the Branch (`git push origin feature/AmazingFeature`)
5. Open a Pull Request



<!-- LICENSE -->
## License

Distributed under the MIT License. See `LICENSE` for more information.



<!-- CONTACT -->
## Contact

Your Name - [@nardogamedev](https://twitter.com/nardogamedev) - dante dot nardo at outlook.com

Project Link: [https://github.com/DanteNardo/UnityCurve](https://github.com/DanteNardo/UnityCurve)



<!-- ACKNOWLEDGEMENTS -->
## Acknowledgements
* [Best-README-Template](https://github.com/othneildrew/Best-README-Template)





<!-- MARKDOWN LINKS & IMAGES -->
<!-- https://www.markdownguide.org/basic-syntax/#reference-style-links -->
[contributors-shield]: https://img.shields.io/github/contributors/DanteNardo/UnityCurve.svg?style=flat-square
[contributors-url]: https://github.com/DanteNardo/UnityCurve/graphs/contributors
[forks-shield]: https://img.shields.io/github/forks/DanteNardo/UnityCurve.svg?style=flat-square
[forks-url]: https://github.com/DanteNardo/UnityCurve/network/members
[stars-shield]: https://img.shields.io/github/stars/DanteNardo/UnityCurve.svg?style=flat-square
[stars-url]: https://github.com/DanteNardo/UnityCurve/stargazers
[issues-shield]: https://img.shields.io/github/issues/DanteNardo/UnityCurve.svg?style=flat-square
[issues-url]: https://github.com/DanteNardo/UnityCurve/issues
[license-shield]: https://img.shields.io/github/license/DanteNardo/UnityCurve.svg?style=flat-square
[license-url]: https://github.com/DanteNardo/UnityCurve/blob/master/LICENSE.txt
[linkedin-shield]: https://img.shields.io/badge/-LinkedIn-black.svg?style=flat-square&logo=linkedin&colorB=555
[linkedin-url]: https://linkedin.com/in/dante-nardo
[product-screenshot]: images/screenshot.png
