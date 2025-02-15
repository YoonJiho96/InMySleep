// import { connectDB } from './dbConnection';
import pool from './dbConnection';
import { FieldPacket } from 'mysql2';
import { contractAddress } from '../config';
import web3 from 'web3';

export const saveMintedNFTToDB = async (receipt: any, userId: string, address: string, tokenURI: string) => {
  const connection = await pool.getConnection();
    try {
      const [metadata_id]: [any[], FieldPacket[]] = await connection.query('SELECT id FROM metadata WHERE metadata_uri = ?', [tokenURI]);
      const token_id = web3.utils.hexToNumber(receipt.logs[1].data);
      const [rows] = await connection.query('INSERT INTO nft (token_id, contract_address, owner_address, transaction_hash, metadata_id, user_id) VALUES (?, ?, ?, ?, ?, ?)', [token_id, contractAddress, address, receipt.transactionHash, metadata_id[0].id, userId]);
      console.log('Saved minted NFT to DB:', rows);
    } catch (error) {
      console.error('Error saving minted NFT to DB:', error);
      throw error;
    } finally {
      connection.release();
    }
  }
  
  export const saveBurnedNFTToDB = async (tokenId: number) => {
    try {
      const [rows] = await pool.query('DELETE FROM nft WHERE token_id = ?', [tokenId]);
      console.log('Saved burned NFT to DB:', rows);
    } catch (error) {
      console.error('Error saving burned NFT to DB:', error);
      throw error;
    }
  }

  export const saveMetadataToDB = async (tokenId: number, metadataURI: string) => {
    const connection = await pool.getConnection();
    try {
      const [metadata_id]: [any[], FieldPacket[]] = await connection.query('SELECT id FROM metadata WHERE metadata_uri = ?', [metadataURI]);
      const [rows] = await connection.query('UPDATE nft SET metadata_id = ? WHERE token_id = ?', [metadata_id[0].id, tokenId]);
      console.log('Saved metadata to DB:', rows);
    } catch (error) {
      console.error('Error saving metadata to DB:', error);
      throw error;
    } finally {
      connection.release();
    }
  }
  
  export const getMetadataFromDB = async (tokenId: number): Promise<string> => {
    try {
      const [rows]: [any[], FieldPacket[]] = await pool.query('SELECT metadata_uri FROM metadata WHERE id = (SELECT metadata_id FROM nft WHERE token_id = ?)', [tokenId]);
      return rows[0].metadata_uri;
    } catch (error) {
      console.error('Error getting metadata from DB:', error);
      throw error;
    }
  }
  
  export const getNFTsFromDB = async (address: string): Promise<string[]> => {
    try {
      const query = `
      SELECT
        metadata.*,
        nft.token_id,
        nft.transaction_hash,
        nft.contract_address
      FROM
        metadata
      JOIN
        nft
      ON
        metadata.id = nft.metadata_id
      WHERE
        nft.owner_address = ?;
      `
      const [rows]: [any[], FieldPacket[]] = await pool.query(query, [address]);
      return rows
    } catch (error) {
      console.error('Error getting NFTs from DB:', error);
      throw error;
    }
  }