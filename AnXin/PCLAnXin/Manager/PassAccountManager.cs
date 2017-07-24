using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using Xamarin.Forms;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace PCLAnXin
{
	public static class PassAccountManager
	{
		private const int ExpiryMin = 1;
		private const bool cachingEnabled = true;

		public async static Task<Tuple<int, PassAccountStatus>> StorePassAccount(string accountName, string loginName, string	anXin, string comments)
		{
			int userId = PassAccountManager.CurrentUserId;
			if (userId > 0) {
				var cryptoHelper = DependencyService.Get<ICryptoHelper> ();
				var enCryptedAnXin = cryptoHelper.EncryptText (anXin);
	
				var scopeId = PassAccountRepository.StorePassAccount (userId, accountName, loginName, enCryptedAnXin, comments);
				if (scopeId > -1) {
					return new Tuple<int, PassAccountStatus> (scopeId, PassAccountStatus.Ok);
				} else {
					return new Tuple<int, PassAccountStatus> (scopeId, PassAccountStatus.Error);
				}
			} else {
				return new Tuple<int, PassAccountStatus> (-1, PassAccountStatus.AuthError);
			}
		}


		public async static Task<Tuple<List<TblPassAccount>, PassAccountStatus>> GetPassAccountList(){
			int userId = PassAccountManager.CurrentUserId;
			if (userId > 0) {
				var response = PassAccountRepository.GetPassAccountList (userId);
				if (response != null) {
					var decryptedResponse = Decrypt (response);
					return Tuple.Create<List<TblPassAccount>,PassAccountStatus> (decryptedResponse, PassAccountStatus.Ok);
				} else if (response == null) {
					return Tuple.Create (default(List<TblPassAccount>), PassAccountStatus.Ok);	
				} else {
					return Tuple.Create (default(List<TblPassAccount>), PassAccountStatus.Error);	
				}
			} else {
				return Tuple.Create (default(List<TblPassAccount>), PassAccountStatus.AuthError);	
			}
		}


		private static List<TblPassAccount> Decrypt(List<TblPassAccount> passAccountList){
			var cryptoHelper = DependencyService.Get<ICryptoHelper> ();
			var itemList = new List<TblPassAccount> ();

			foreach (var item in passAccountList) {
				itemList.Add (
					new TblPassAccount {
						AccountName = item.AccountName,
						LoginName = item.LoginName,
						AnXin = cryptoHelper.DecryptText (item.AnXin),
						Comments = item.Comments,
						UserId = item.UserId,
						EncryptedPass = item.AnXin,
						ID = item.ID,
					});

			}
			return itemList;
		}


		public static PassAccountStatus DelPassAccount(int id){

			int userId = PassAccountManager.CurrentUserId;
			if (userId > 0) {
				var affected = PassAccountRepository.DelPassAccount (userId, id);
				if (affected > -1) {
					return PassAccountStatus.Ok;
				} else {
					return PassAccountStatus.Error;
				}
			} else {
				return PassAccountStatus.AuthError;
			}
		}


		public async static Task<Tuple<int, PassAccountStatus>>  UpdatePassAccount(int Id, string accountName, string loginName, string	anXin, string comments)
		{
			int userId = PassAccountManager.CurrentUserId;
			if (userId > 0) {
				var cryptoHelper = DependencyService.Get<ICryptoHelper> ();
				var enCryptedAnXin = cryptoHelper.EncryptText (anXin);
				var affected = PassAccountRepository.UpdatePassAccount (Id, userId, accountName, loginName, enCryptedAnXin, comments);
				if (affected > -1) {
					return new Tuple<int, PassAccountStatus> (affected, PassAccountStatus.Ok);
				} else {
					return new Tuple<int, PassAccountStatus> (affected, PassAccountStatus.Error);
				}
			} else {
				return new Tuple<int, PassAccountStatus> (-1, PassAccountStatus.AuthError);
			}
		}

		#region masterPass

		public static int CurrentUserId;

		private async static Task<Tuple<int, PassAccountStatus>> StoreMaster(string masterPass){
			var cryptoHelper = DependencyService.Get<ICryptoHelper> ();
			var enCryptedMasterPass = cryptoHelper.EncryptText (masterPass);

			var scopeId = UserAccountRepository.StoreMasaterPass(enCryptedMasterPass);
			if (scopeId > -1) {
				return new Tuple<int, PassAccountStatus> (scopeId, PassAccountStatus.Ok);
			} else {
				return new Tuple<int, PassAccountStatus> (scopeId, PassAccountStatus.Error);
			}
		}

		private async static Task<Tuple<TblUser, PassAccountStatus>> GetMaster(string masterPass){
			var userData = UserAccountRepository.GetMasaterPass (masterPass);
			if (userData != null) {
				return Tuple.Create<TblUser,PassAccountStatus> (userData, PassAccountStatus.Ok);
			} else {
				return Tuple.Create (default(TblUser), PassAccountStatus.Error);	
			}
		}

		public async static Task<int> Validate(string masterPass){
			if (!string.IsNullOrEmpty (masterPass)) {
				var cryptoHelper = DependencyService.Get<ICryptoHelper> ();
				string decryptedMasterPass = cryptoHelper.EncryptText (masterPass.Trim());
				var result = await GetMaster (decryptedMasterPass);
				if (result.Item2 == PassAccountStatus.Ok && result.Item1 != null) {
					CurrentUserId = result.Item1.ID;
					return result.Item1.ID; //return user id. 
				}
			}
			CurrentUserId = 0;
			return 0;
		}

		public async static Task<bool> CreateUser(string masterPass){
			var result = await StoreMaster (masterPass);
			if (result.Item2 == PassAccountStatus.Ok && result.Item1 != null) {
				return true;
			} else {
				return false;
			}
		}
		#endregion

	}

	public enum PassAccountStatus  {
		Ok, 
		Error,
		AuthError,
		Offline
	}
}

