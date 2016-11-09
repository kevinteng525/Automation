require 'watir-webdriver'
require_relative '..\Variables\Variables_Configure.rb'
require_relative '..\Objects\DM_Objects.rb'
require_relative '..\Objects\Login_page.rb'
require_relative '..\Objects\Main_Matter_page.rb'
require_relative '..\Objects\New_Matter_page.rb'
require_relative '..\Objects\Matter_Tree_page.rb'
require_relative '..\Objects\New_Search_page.rb'
require_relative '..\Objects\DM_Main_Head_page.rb'
require_relative '..\Objects\CollectionArea_page.rb'
require_relative '..\Objects\Search_Result_page.rb'
require_relative '..\Objects\Export_Setting_page.rb'
require_relative '..\Objects\Export_Summary_page.rb'
require_relative '..\Objects\Main_Identity_page.rb'
require_relative '..\Objects\New_Identity_page.rb'
require_relative '..\Objects\Delete_Identity_page.rb'
require_relative '..\Objects\Delete_Matter_page.rb'
require_relative '..\Objects\Main_Administration_page.rb'
require_relative '..\Objects\Find_User_List_page.rb'
require_relative '..\Objects\Select_User_Add_page.rb'
require_relative '..\Objects\Delete_User_page.rb'
require_relative '..\Objects\All_Items_page.rb'
require_relative '..\Objects\edit_matterproperties.rb'
gem "minitest"
require 'minitest/autorun'
require_relative '..\Objects\minitest_saber_base.rb'
require_relative '..\Objects\saber_base.rb'
#Smoke test cases.
class TestSuite 
    i=0
    while true do 
        begin
          dmweb1 = Site.new(Watir::Browser.new) 
          dmweb1.login_page.open($DMWebSite)
          dmweb1.login_page.login_as($es1service, $password)
          dmweb1.usermenu_page.clickAdministration
          if dmweb1.main_administration_page.verifyUserInGrid($deleteusername)
            puts 'The user has been added'
            break
          elsif i==10
            puts 'Failed to add the user after 10 tries.'
            break
          end
          dmweb1.usermenu_page.clickAdministration
          sleep(10)
          dmweb1.main_administration_page.add_user_click
          dmweb1.select_user_add_page.set_UserName($username)
          dmweb1.select_user_add_page.FindUser
          dmweb1.find_user_list_page.click_FindUserList($username)
          dmweb1.find_user_list_page.click_OK
          dmweb1.select_user_add_page.click_OK
          sleep(3)
          dmweb1.main_administration_page.checkall_click
          sleep(6)
        rescue
          puts "error:#{$!} at:#{$@}"
        ensure
           dmweb1.close
           i=i+1
        end
    end
 end   

  

 

