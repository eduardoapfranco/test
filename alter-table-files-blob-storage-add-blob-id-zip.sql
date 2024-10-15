ALTER TABLE `files_blob_storage` 
ADD COLUMN `zip` TINYINT NULL DEFAULT 0 AFTER `blob_id`;
