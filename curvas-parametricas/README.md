# Trajetória com Curva Paramétrica

---

## Descrição do Projeto

> Este jogo foi desenvolvido como parte da disciplina *Animação Computadorizada* com o objetivo de explorar trajetorias de diferentes curvas, Linear, Catmull-Rom e Bezier.

---

## Estrutura do Projeto


| Arquivo               | Descrição                                                  |
|-----------------------|------------------------------------------------------------|
| `CatmullRomCurve.cs`  | Implementa a curva de Catmull-Rom.|
| `Curve3D.cs`          | |
| `LinearCurve.cs`      | Implementa interpolação linear entre pares consecutivos de pontos de controle.|
| `BezierCurve.cs`      | Implementa a curva de Bézier cúbica.|
| `CurveManager.cs`     | Gerencia as curvas e a troca entre os modos.|
| `Pointer.cs`          | Personagem que percorre a curva ativa ponto a ponto.|
| `Main.cs`             | Script raiz da cena. |

---
## Informações Técnicas

- **Engine:** Godot 4.7 Mono  
- **Linguagem:** C#  
- **Dependências:** .NET  
- **Plataforma-alvo:** Desktop (Windows/Linux)

---

## Checklist de Requisitos

- [x]   Estrutura de pontos de controle que possam ser adicionados. (No editor)
- [x]   Implementação de curva parametrica que não sejá linear ou Catmull-Rom. (Bezier)
- [x]   Permitir visualização das curvas implementadas. (Alternancia entre curvas com TAB)

---

## Link para a Build

🔗 [https://usuario.itch.io/nome-do-jogo](https://usuario.itch.io/nome-do-jogo)

---

## Referências e/ou créditos

 - Repositorio de exemplo da disciplina [AC2026-02](https://github.com/fellowsheep/AC2026-2/tree/main/exemplo-interpolacao)
