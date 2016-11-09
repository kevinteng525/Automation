require 'watir-webdriver'
require_relative '..\Variables\Variables_Configure.rb'
require_relative '..\Objects\DM_Objects.rb'
gem "minitest"
require "minitest/autorun"

#Main Matter page====================================
class TagManger < BrowserContainer
  def clickTagManager
    tagManger_Button.wait_until_present
    tagManger_Button.click
    @browser.wait(3)
  end
  def clickNewButton
    click_new_button.wait_until_present
    click_new_button.click
  end
  def clickEditButton
    click_Edit_button.wait_until_present
    click_Edit_button.click
  end
  def clickDeleteButton
    click_Delete_button.wait_until_present
    click_Delete_button.click
  end
  #delete tag in matter properties
  def clickDeleteButton1
    click_Delete_button1.wait_until_present
    click_Delete_button1.click
  end
  def clickDefaultTag(tagname)
    click__Default_Tag(tagname).wait_until_present
    click__Default_Tag(tagname).click
  end
  def click__Default_Tag(tagname)
       if @browser.div(:id=>'gridx_Grid_1').span(:title=>tagname).exist?
          @browser.div(:id=>'gridx_Grid_1').span(:title=>tagname)
       else
          @browser.div(:id=>'uniqName_2_0').span(:title=>tagname).exist?
          @browser.div(:id=>'uniqName_2_0').span(:title=>tagname)
       end 
  end
  #focus to tag in matter properties
  def focusToTags
    @browser.send_keys :right
    @browser.send_keys :left
    @browser.send_keys :tab
    @browser.send_keys :tab
    @browser.send_keys :space
  end
  
 private
  def click_Delete_button
      @browser.span(:id=>'dijit_form_Button_2_label')
  end
  # delete the tag in matter properties
  def click_Delete_button1
      @browser.span(:id=>'dijit_form_Button_9_label')
  end
  
  
  def click_Edit_button
      @browser.span(:id=>'dijit_form_Button_1_label')
  end
  def click_new_button
      @browser.span(:id=>'dijit_form_Button_0_label')
  end
  def tagManger_Button
      @browser.span(:id=>'AdminTabs_tablist_AdminTab_TagManager')
  end
  
end