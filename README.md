## Introducción
En este reporte, se explican el desarrollo e implementación de diferentes patrones de disparo en Unity, controlado por un objeto denominado jugador. El sistema utiliza programación orientada a objetos y componentes propios de Unity para generar, gestionar y contabilizar balas en pantalla. A continuación, se explica la lógica técnica de cada patrón implementado, se justifica su diferencia conceptual y se reflexiona sobre los retos enfrentados y los aprendizajes obtenidos del nivel fácil.

## Implementación Técnica de los Patrones

### 1. Patrón RingSpin (Círculo giratorio)
Este patrón genera un anillo completo de balas alrededor del jugador. Para ello primero se calculan los ángulos equidistantes entre cada bala (360 / númeroDeBalas) y se emite cada proyectil en la posición del jugador y se le asigna una dirección radial usando rotaciones con Quaternion.Euler. Tras cada iteración, el ángulo base se incrementa progresivamente, lo que genera la ilusión de un círculo giratorio. 

### 2. Patrón SweepingLines (Líneas rectas)
En este patrón se simula un conjunto de emisores virtuales con desfases de fase para generar un efecto de barrido. Cada emisor dispara varias balas en línea recta hacia la derecha, generando un barrido visual característico. La función senoidal (Mathf.Sin) se calcula para cada emisor, pero actualmente no afecta la posición de disparo, por lo que todos los proyectiles se originan desde la posición del jugador.

### 3. Patrón TurnMidFlight (Cambio en vuelo)
Este patrón dispara abanicos de proyectiles en dirección recta que, tras un tiempo predefinido, cambian de trayectoria. Se implementa mediante un componente auxiliar (BulletTurning) que inicia la bala con una velocidad inicial. Tras un retraso (turnAfterSeconds), se aplica una rotación al vector velocidad (Quaternion.Euler) para desviar su curso. El resultado es un cono inicial que cambia de direcciones después de ciertos segundos, cambiando la trayectoria final.
Justificación de las Diferencias entre Patrones
Cada patrón es diferente tanto en su lógica de implementación como en su efecto visual y jugable. Primero, Círculo giratorio muestra la simetría circular y el movimiento continuo, mientras Líneas rectas introducen el uso de emisores móviles en línea recta al lado derecho desde el jugador. Cambio en vuelo rompe la linealidad al añadir un cambio de trayectoria inesperada, aumentando la dificultad y la imprevisibilidad. Estas diferencias permiten ofrecer variedad al jugador y demostrar el dominio de diversas técnicas matemáticas y de programación en Unity.

## Retos y Aprendizajes
Durante el desarrollo se enfrentaron diversos retos. Primero, tuve problemas con la gestión de proyectiles activos y fue necesario implementar la clase BulletManager para contabilizar cuántas balas permanecen activas en escena, evitando desbordes de memoria y facilitando la depuración. Además había problemas de sincronización temporal. Para eso, se utilizó WaitForSeconds en corrutinas para controlar la cadencia de disparo y coordinar el cambio de patrones. Para los problemas de visibilidad y destrucción de objetos, se debió implementar lógica para detectar cuando una bala sale de la cámara y destruirla adecuadamente, evitando errores de referencias nulas. Últimamente, para el control del jugador, la integración de las teclas de movimiento implicó asegurarse de que los disparos siguieran apareciendo desde la posición dinámica del jugador. Los principales aprendizajes incluyen el uso de corrutinas para crear comportamientos temporales complejos, la importancia de un manejo robusto de objetos instanciados, y el diseño modular de patrones reutilizables.

## Conclusión
La implementación de los tres patrones descritos demuestra la aplicación de conceptos fundamentales de programación en Unity: uso de transformaciones geométricas, corrutinas para temporización, control de objetos instanciados y comunicación entre componentes. Cada patrón aporta una dinámica distinta al sistema, y la combinación de ellos evidencia el aprendizaje alcanzado en el diseño de mecánicas de juego.

## Recursos
Video demostrativo: https://drive.google.com/file/d/1_fHV1xn6xxPlSXmjDshI3rtFzRqwnO4B/view?usp=sharing

Repositorio en GitHub: https://github.com/dongjumun01/Evidencia_2_Actividad_Integradora_Graficas_Nivel_Facil.git

