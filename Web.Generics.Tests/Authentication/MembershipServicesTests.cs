/*
Copyright 2010 Inspira Tecnologia.
All Rights Reserved.

Contact: Thiago Alves <thiago.alves@inspira.com.br>

This file is part of Web.Generics

Web.Generics is free software: you can redistribute it and/or modify
it under the terms of the GNU Lesser General Public License as published by
the Free Software Foundation, either version 3 of the License, or
(at your option) any later version.

Web.Generics is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU Lesser General Public License for more details.

You should have received a copy of the GNU Lesser General Public License
along with Web.Generics.  If not, see <http://www.gnu.org/licenses/>.
*/

﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NHibernate;
using NHibernate.Criterion;
using Web.Generics.ApplicationServices.Identity;
using Inspira.Blog.DomainModel;
using Inspira.Blog.Infrastructure.DataAccess.Repositories;
using System.Reflection;

namespace Web.Generics.Tests.Authentication
{
    [TestClass]
    public class MembershipServicesTests
    {
        public MembershipServicesTests() {
            ContextFactory.InitializeAppManager();
            session = ApplicationManager.SessionFactory.OpenSession();
        }

        ISession session;
        UserRepository userRepository;
        IdentityService<User> identityService;
        User user;
        User user2;
        String password;

        [TestInitialize]
        public void Initialize() {
            session.BeginTransaction();
            userRepository = new UserRepository(session);
            identityService = new IdentityService<User>(userRepository);

            password = "neoistheone";
            
            user = new User
            {
                Name = "John Doe",
                BirthDate = new DateTime(1982, 8, 1),
                Email = "john.doe@inspira.com.br",
                Username = "john_doe",
                Address = new Address { City = "São paulo", StreetName = "name", Number = "123B", State = "SP", ZipCode = "03423-234" }
            };

            user2 = new User
            {
                Name = "Fulano",
                BirthDate = DateTime.Now,
                Email = "fulano@inspira.com.br",
                Password = "$$$$",
                Username = "john_doe",
                Address = new Address { City = "São paulo", StreetName = "name", Number = "123B", State = "SP", ZipCode = "03423-234" }
            };
        }

        /* Register: */
        [TestMethod]
        public void Register_with_valid_data_inserts_user_into_database_and_returns_success()
        {
            Assert.AreEqual(RegisterStatus.Success, identityService.Register(user, u => u.Username, u => u.Email, (s) => user.Password = s, password));
        }

        [TestMethod]
        public void Register_with_existing_username_returns_ExistingUsername_state()
        {            
            userRepository.SaveOrUpdate(user);
            Assert.AreEqual(RegisterStatus.UsernameAlreadyExists, identityService.Register(user2, u => u.Username, u => u.Email, (s) => user2.Password = s, "minhasenha"));
        }

        // Register with existing e-mail returns ExistingEmail state
        [TestMethod]
        public void Register_with_existing_email_returns_ExistingEmail_state() 
        {
            userRepository.SaveOrUpdate(user);
            Assert.AreEqual(RegisterStatus.EmailAlreadyExists, identityService.Register(user2, u => u.Username, u => u.Email, (s) => user2.Password = s, "minhasenha"));
        }

        [TestMethod]
        public void Register_with_invalid_data_returns_invalid_state() //What's the criteria?
        {
            throw new NotImplementedException();
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Register_with_null_data_throws_argumentnullexception()
        {
            user = new User();
            identityService.Register(user, u => u.Username, u => u.Email, (s) => user.Password = s, password);
        }

        [TestMethod]
        public void Register_with_null_role_throws_argumentNullException()
        {
            throw new NotImplementedException();
        }

        [TestMethod]
        public void Register_fail_must_rollback_transaction() 
        {
            throw new NotImplementedException();
        }
  
         // Validate:
        [TestMethod]
        public void Validate_existing_user_with_correct_password_returns_true()
        {
            Assert.AreEqual(RegisterStatus.Success, identityService.Register(user, u => u.Username, u => u.Email, (s) => user.Password = s, password));
            Assert.IsTrue(identityService.Validate("john_doe", password));
        }

        [TestMethod]
        public void Validate_existing_user_with_incorrect_password_returns_false()
        {
            Assert.AreEqual(RegisterStatus.Success, identityService.Register(user, u => u.Username, u => u.Email, (s) => user.Password = s, password));
            Assert.IsFalse(identityService.Validate("john_doe", "something"));
        }

        [TestMethod]
        public void Validate_non_existing_user_returns_false()
        {
            Assert.AreEqual(RegisterStatus.Success, identityService.Register(user, u => u.Username, u => u.Email, (s) => user.Password = s, password));
            Assert.IsFalse(identityService.Validate("huckleberry finn", "something"));
        }

         // ChangePassword
        [TestMethod]
        public void User_changing_password_with_correct_password_and_valid_new_password_returns_success()
        {
            Assert.AreEqual(RegisterStatus.Success, identityService.Register(user, u => u.Username, u => u.Email, (s) => user.Password = s, password));        
            Assert.AreEqual(PasswordChangeStatus.Success, identityService.ChangePassword("john_doe", password, "newPassword"));
        }

        [TestMethod]
        public void Admin_changing_password_with_valid_new_password_returns_Success()
        {
            Assert.AreEqual(RegisterStatus.Success, identityService.Register(user, u => u.Username, u => u.Email, (s) => user.Password = s, password));
            Assert.AreEqual(PasswordChangeStatus.Success, identityService.AdministrativePasswordChange("john_doe", "newPassword"));
        }

         /*    - Admin changing password with invalid new password returns InvalidCurrentPassword*/

        [TestMethod]
        public void User_changing_password_with_incorrect_current_password_returns_IncorrectCurrentPassword() 
        {
            Assert.AreEqual(RegisterStatus.Success, identityService.Register(user, u => u.Username, u => u.Email, (s) => user.Password = s, password));
            Assert.AreEqual(PasswordChangeStatus.InvalidCurrentPassword, identityService.ChangePassword("john_doe", "wrongPassword", "newPassword"));
        }

         /*    - User changing password with correct current password and invalid new password returns InvalidNewPassword
         * ResetPassword:*/
        [TestMethod]
        public void Password_reset_for_existing_user_changes_password_and_returns_it()
        {
            Assert.AreEqual(RegisterStatus.Success, identityService.Register(user, u => u.Username, u => u.Email, (s) => user.Password = s, password));
            Assert.IsNotNull(identityService.ResetPassword("john_doe"));
        }

        [TestMethod]
        public void Password_reset_for_non_existing_user_returns_null() 
        {
            Assert.IsNull(identityService.ResetPassword("idontexist"));
        }

         /* ResetPasswordWithValidationKey:*/
        [TestMethod]
        public void Password_reset_for_existing_user_with_valid_validation_key_changes_password_and_returns_it()
        {
            Assert.AreEqual(RegisterStatus.Success, identityService.Register(user, u => u.Username, u => u.Email, (s) => user.Password = s, password));
            string validationKey = identityService.GenerateValidationKey("john.doe@inspira.com.br");
            Assert.IsNotNull(validationKey);
            Assert.IsNotNull(identityService.ResetPasswordWithValidationKey("john_doe", validationKey));
        }

        [TestMethod]
        public void Password_resetfor_existing_user_with_invalid_validation_key_returns_null()
        {
            Assert.AreEqual(RegisterStatus.Success, identityService.Register(user, u => u.Username, u => u.Email, (s) => user.Password = s, password));

            string validationKey = identityService.GenerateValidationKey("john.doe@inspira.com.br");

            Assert.IsNotNull(validationKey);

            Assert.IsNull(identityService.ResetPasswordWithValidationKey("john_doe", "something else"));
        }

        [TestMethod]
        public void Password_reset_with_validation_key_for_non_existing_user_returns_null()
        {
            Assert.IsNull(identityService.ResetPasswordWithValidationKey("idontexist", "something else"));
        }

        // GenerateValidationKey:
        [TestMethod]
        public void GenerateValidationKey_for_existing_email_generates_key_and_returns_it()
        {
            try
            {
                Assert.AreEqual(RegisterStatus.Success, identityService.Register(user, u => u.Username, u => u.Email, (s) => user.Password = s, password));
                Assert.IsNotNull(identityService.GenerateValidationKey("john.doe@inspira.com.br"));
            }
            catch
            {
                throw;
            }
        }

        [TestMethod]
        public void GenerateValidationKey_for_nonexisting_email_returns_null()
        {
            Assert.IsNull(identityService.GenerateValidationKey("idontexist@inspira.com.br"));
        }

         /* Unlock:
         *     - Unlock verify if the user exists and update lock flag to false
         * Lock:
         *     - Unlock verify if the user exists and update lock flag to true
         * 
         */

        [TestCleanup]
        public void Cleanup()
        {
            try
            {
                session.Transaction.Commit();
            }
            catch
            {
                session.Transaction.Rollback();
            }
            session.Flush();
            session.Clear();
        }
    }
}
