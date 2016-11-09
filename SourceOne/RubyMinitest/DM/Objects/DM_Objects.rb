require 'watir-webdriver'
gem "minitest"
require "minitest/autorun"

#root browser class.
class BrowserContainer
  def initialize(browser)
    @browser = browser
  end
end

class Site < BrowserContainer
  
  
  #This is a page of login.
  def login_page
    @login_page = LoginPage.new(@browser)
  end

  #this is a icon in the top right main page.
  def useraccount_Page
    @useraccount_Page = UserAccountPage.new(@browser)
  end
  
  #this is the menu bar in the top main page
  def usermenu_page
    @usermenu_page = UserMenuPage.new(@browser)
  end

  #this is a page after click on Matter tab in the main page.
  def main_matter_page
    @main_matter_page = MainMatterPage.new(@browser)
  end

  #this is a page of New Matter dialog box.
  def new_matter_page
    @new_matter_page = NewMatterPage.new(@browser)
  end

  #This is the pane of matter tree in the left main page.
  def matter_tree_page
    @matter_tree_page = MatterTreePage.new(@browser)
  end
  
  #this is a dialog box after click on New Search button.
  def new_search_page
    @new_search_page = NewSearchPage.new(@browser)
  end   

  #this is the head pane includes Matters/Identities/Administration tab.
  def main_head_page
    @main_head_page = MainHeadPage.new(@browser)
  end  

  #this is a page of search summary
  def search_summary_page
    @search_summary_page = SearchSummaryPage.new(@browser)
  end  
  
  #this is a page of search summary
  def search_result_page
    @search_summary_page = SearchResultPage.new(@browser)
  end

  #this is a dialog box of export setting.
  def export_setting_page
    @export_setting_page = ExportSettingPage.new(@browser)
  end
  
  #this is a page of export summary page.
  def export_summary_page
    @export_summary_page = ExportSummaryPage.new(@browser)
  end
  
  #this is a page of identity page.
  def main_identity_page
    @main_identity_page = MainIdentityPage.new(@browser)
  end
  
  #this is a page of new identity page
  def new_identity_page
    @new_identity_page = NewIdentityPage.new(@browser)
  end
  
  #this is a page of delete identity page
  def delete_identity_page
    @delete_identity_page = DeleteIdentityPage.new(@browser)
  end
  
  #this is a page after click on Administration tab in the main page.
  def main_administration_page
    @main_matter_page = MainAdministrationPage.new(@browser)
  end
  
  #this is a page of select user which needs you ito nput add user's name
  def select_user_add_page
    @select_user_add_page = SelectUserAddPage.new(@browser)
  end
  
  #this is a page of find user list.
  def find_user_list_page
    @find_user_list_page = FindUserListPage.new(@browser)
  end
  
  #this is a page of delete user page
  def delete_user_page
    @delete_user_page = DeleteUserPage.new(@browser)
  end
  
  #this is a page of all items results page.
  def allItems_result_page
    @allItems_result_page = AllItemsPage.new(@browser)
  end  
  
   #this is BtnIDHelper.
  def btn_id_helper
    @btn_id_helper = BtnIDHelper.new(@browser)
  end 
  #This is DeleteMatterPage
  def Delete_Matter_page
    @Delete_Matter_page = DeleteMatterPage.new(@browser)
  end  
  #this is a function of close browser.
  def close
    @browser.close
  end
   #this is a function of TagManager.
  def Tag_Manger
    @Tag_Manger=TagManger.new(@browser)
  end
  #this is a matter tree items.
  def Mattertree_items
    @Mattertree_items=Mattertreeitems.new(@browser)
  end
  
  
  def Edit_matterproperties
    @Edit_matterproperties=Editmatterproperties.new(@browser)
  end
  
  #new matter dialog 
  def NewMatter_Dialog
    @NewMatter_Dialog=NewMatterDialog.new(@browser)
  end
  
   #matter properties dialog 
  def Matter_Properties_Dialog
    @Matter_Properties_Dialog=MatterPropertiesDialog.new(@browser)
  end
end 


