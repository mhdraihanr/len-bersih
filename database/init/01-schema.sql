-- Len Bersih Database Schema
-- Compatible with existing lenbersih.sql structure

SET SQL_MODE = "NO_AUTO_VALUE_ON_ZERO";
SET time_zone = "+00:00";

/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!40101 SET NAMES utf8mb4 */;

-- Use the database created by MARIADB_DATABASE environment variable
USE lenbersih;

-- --------------------------------------------------------
-- Table structure for table `groups`
-- --------------------------------------------------------

DROP TABLE IF EXISTS `groups`;
CREATE TABLE `groups` (
  `id` mediumint(8) UNSIGNED NOT NULL AUTO_INCREMENT,
  `name` varchar(20) NOT NULL,
  `description` varchar(100) NOT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_general_ci;

INSERT INTO `groups` (`id`, `name`, `description`) VALUES
(1, 'admin', 'Administrator'),
(2, 'members', 'General User');

-- --------------------------------------------------------
-- Table structure for table `users`
-- --------------------------------------------------------

DROP TABLE IF EXISTS `users`;
CREATE TABLE `users` (
  `id` int(11) UNSIGNED NOT NULL AUTO_INCREMENT,
  `ip_address` varchar(45) NOT NULL,
  `username` varchar(100) DEFAULT NULL,
  `password` varchar(255) NOT NULL,
  `email` varchar(254) NOT NULL,
  `activation_selector` varchar(255) DEFAULT NULL,
  `activation_code` varchar(255) DEFAULT NULL,
  `forgotten_password_selector` varchar(255) DEFAULT NULL,
  `forgotten_password_code` varchar(255) DEFAULT NULL,
  `forgotten_password_time` int(11) UNSIGNED DEFAULT NULL,
  `remember_selector` varchar(255) DEFAULT NULL,
  `remember_code` varchar(255) DEFAULT NULL,
  `created_on` int(11) UNSIGNED NOT NULL,
  `last_login` int(11) UNSIGNED DEFAULT NULL,
  `active` tinyint(1) UNSIGNED DEFAULT NULL,
  `first_name` varchar(50) DEFAULT NULL,
  `last_name` varchar(50) DEFAULT NULL,
  `company` varchar(100) DEFAULT NULL,
  `phone` varchar(20) DEFAULT NULL,
  PRIMARY KEY (`id`),
  UNIQUE KEY `uc_email` (`email`),
  UNIQUE KEY `uc_activation_selector` (`activation_selector`),
  UNIQUE KEY `uc_forgotten_password_selector` (`forgotten_password_selector`),
  UNIQUE KEY `uc_remember_selector` (`remember_selector`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_general_ci;

-- Default admin user: username=administrator, password=admin123
INSERT INTO `users` (`id`, `ip_address`, `username`, `password`, `email`, `activation_selector`, `activation_code`, `forgotten_password_selector`, `forgotten_password_code`, `forgotten_password_time`, `remember_selector`, `remember_code`, `created_on`, `last_login`, `active`, `first_name`, `last_name`, `company`, `phone`) VALUES
(1, '127.0.0.1', 'administrator', '$2y$10$uoFuEnIjm2j6XvOFou9Nx.BqHbzFxTJNlulFW5FvgDl70eC1T0e5y', 'admin@admin.com', NULL, '', NULL, NULL, NULL, NULL, NULL, 1268889823, UNIX_TIMESTAMP(), 1, 'Admin', 'istrator', 'ADMIN', '0');

-- --------------------------------------------------------
-- Table structure for table `users_groups`
-- --------------------------------------------------------

DROP TABLE IF EXISTS `users_groups`;
CREATE TABLE `users_groups` (
  `id` int(11) UNSIGNED NOT NULL AUTO_INCREMENT,
  `user_id` int(11) UNSIGNED NOT NULL,
  `group_id` mediumint(8) UNSIGNED NOT NULL,
  PRIMARY KEY (`id`),
  UNIQUE KEY `uc_users_groups` (`user_id`,`group_id`),
  KEY `fk_users_groups_users1_idx` (`user_id`),
  KEY `fk_users_groups_groups1_idx` (`group_id`),
  CONSTRAINT `fk_users_groups_groups1` FOREIGN KEY (`group_id`) REFERENCES `groups` (`id`) ON DELETE CASCADE ON UPDATE NO ACTION,
  CONSTRAINT `fk_users_groups_users1` FOREIGN KEY (`user_id`) REFERENCES `users` (`id`) ON DELETE CASCADE ON UPDATE NO ACTION
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_general_ci;

INSERT INTO `users_groups` (`id`, `user_id`, `group_id`) VALUES
(7, 1, 1),
(8, 1, 2);

-- --------------------------------------------------------
-- Table structure for table `status`
-- --------------------------------------------------------

DROP TABLE IF EXISTS `status`;
CREATE TABLE `status` (
  `id_status` int(11) NOT NULL AUTO_INCREMENT,
  `status` varchar(100) NOT NULL,
  PRIMARY KEY (`id_status`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1 COLLATE=latin1_swedish_ci;

INSERT INTO `status` (`id_status`, `status`) VALUES
(1, 'Validasi Laporan Tim Sekretariat DITERIMA'),
(2, 'Validasi Laporan Tim Sekretariat DITOLAK'),
(3, 'Analisa Tim Pengawas DITINDAK LANJUTI'),
(4, 'Analisa Tim Pengawas DIHENTIKAN'),
(5, 'Audit Tim Investigasi TERBUKTI'),
(6, 'Audit Tim Investigasi TIDAK TERBUKTI');

-- --------------------------------------------------------
-- Table structure for table `pelaporan`
-- --------------------------------------------------------

DROP TABLE IF EXISTS `pelaporan`;
CREATE TABLE `pelaporan` (
  `id_pelaporan` int(11) NOT NULL AUTO_INCREMENT,
  `nama` varchar(150) NOT NULL,
  `email` varchar(150) NOT NULL,
  `jenis_laporan` varchar(150) NOT NULL,
  `nama_terlapor` varchar(150) NOT NULL,
  `jabatan_terlapor` varchar(150) NOT NULL,
  `unit_kerja_terlapor` varchar(150) NOT NULL,
  `waktu_kejadian` date NOT NULL,
  `lokasi_kejadian` varchar(150) NOT NULL,
  `uraian` text NOT NULL,
  `dokumen` varchar(255) NOT NULL,
  `kode` varchar(5) NOT NULL,
  `created_date` date NOT NULL,
  `id_status` int(11) NOT NULL DEFAULT 1,
  `approve` int(11) NOT NULL DEFAULT 0,
  `approved_date` datetime NOT NULL,
  PRIMARY KEY (`id_pelaporan`),
  KEY `idx_kode` (`kode`),
  KEY `idx_created_date` (`created_date`),
  KEY `idx_email` (`email`),
  KEY `idx_nama_terlapor` (`nama_terlapor`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1 COLLATE=latin1_swedish_ci;

-- --------------------------------------------------------
-- Table structure for table `history_status`
-- --------------------------------------------------------

DROP TABLE IF EXISTS `history_status`;
CREATE TABLE `history_status` (
  `id_history_status` int(11) NOT NULL AUTO_INCREMENT,
  `id_status` int(11) NOT NULL,
  `id_pelaporan` int(11) NOT NULL,
  `alasan` text NOT NULL,
  `created_date` date NOT NULL,
  PRIMARY KEY (`id_history_status`),
  KEY `idx_id_pelaporan` (`id_pelaporan`),
  KEY `idx_id_status` (`id_status`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1 COLLATE=latin1_swedish_ci;

-- --------------------------------------------------------
-- Table structure for table `dokumen`
-- --------------------------------------------------------

DROP TABLE IF EXISTS `dokumen`;
CREATE TABLE `dokumen` (
  `id_dokumen` int(11) NOT NULL AUTO_INCREMENT,
  `dokumen` varchar(255) NOT NULL,
  `id_pelaporan` int(11) NOT NULL,
  PRIMARY KEY (`id_dokumen`),
  KEY `idx_id_pelaporan` (`id_pelaporan`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1 COLLATE=latin1_swedish_ci;

-- --------------------------------------------------------
-- Table structure for table `login_attempts`
-- --------------------------------------------------------

DROP TABLE IF EXISTS `login_attempts`;
CREATE TABLE `login_attempts` (
  `id` int(11) UNSIGNED NOT NULL AUTO_INCREMENT,
  `ip_address` varchar(45) NOT NULL,
  `login` varchar(100) NOT NULL,
  `time` int(11) UNSIGNED DEFAULT NULL,
  PRIMARY KEY (`id`),
  KEY `idx_ip_address` (`ip_address`),
  KEY `idx_time` (`time`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_general_ci;

-- --------------------------------------------------------
-- AUTO_INCREMENT for tables
-- --------------------------------------------------------

ALTER TABLE `dokumen`
  MODIFY `id_dokumen` int(11) NOT NULL AUTO_INCREMENT;

ALTER TABLE `groups`
  MODIFY `id` mediumint(8) UNSIGNED NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=3;

ALTER TABLE `history_status`
  MODIFY `id_history_status` int(11) NOT NULL AUTO_INCREMENT;

ALTER TABLE `login_attempts`
  MODIFY `id` int(11) UNSIGNED NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=4;

ALTER TABLE `pelaporan`
  MODIFY `id_pelaporan` int(11) NOT NULL AUTO_INCREMENT;

ALTER TABLE `status`
  MODIFY `id_status` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=7;

ALTER TABLE `users`
  MODIFY `id` int(11) UNSIGNED NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=5;

ALTER TABLE `users_groups`
  MODIFY `id` int(11) UNSIGNED NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=9;

COMMIT;

/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;