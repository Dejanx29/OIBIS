﻿using Common;
using System;
using System.Collections.Generic;
using System.ServiceModel;

namespace Contracts
{
    [ServiceContract]
	public interface IWCFContract
	{

		[OperationContract]
		[FaultContract(typeof(Exception))]
		void SendMessage(byte[] msg);

		[OperationContract]
		[FaultContract(typeof(Exception))]
		List<Message> GetAllMessages();

		[OperationContract]
		[FaultContract(typeof(Exception))]
		List<Message> GetAllMessagesThatExceedInSize();

		[OperationContract]
		[FaultContract(typeof(Exception))]
		void BanUser(string name);

		[OperationContract]
		[FaultContract(typeof(Exception))]
		void UnbanUser(string name);
		[OperationContract]
		[FaultContract(typeof(Exception))]
		List<string> GetAllBanedStudents();

	}
}
