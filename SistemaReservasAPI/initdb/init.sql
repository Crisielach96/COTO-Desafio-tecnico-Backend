DROP TABLE IF EXISTS `Reservas`;
CREATE TABLE `Reservas` (
  `id` int NOT NULL AUTO_INCREMENT,
  `salon_id` varchar(45) NOT NULL,
  `cliente` varchar(45) NOT NULL,
  `fecha` date NOT NULL,
  `hora_inicio` varchar(45) NOT NULL,
  `hora_fin` varchar(45) NOT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=14 DEFAULT CHARSET=utf8mb3;