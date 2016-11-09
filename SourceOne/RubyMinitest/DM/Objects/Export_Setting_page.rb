require 'watir-webdriver'
require_relative '..\Variables\Variables_Configure.rb'
require_relative '..\Objects\DM_Objects.rb'
require_relative '..\Objects\BtnIDHelper.rb'
gem "minitest"
require "minitest/autorun"

class ExportSettingPage < BrowserContainer
  
  #select a value for Container type.
  def selectContainerType(type1)
    if @browser.span(:text=>'Export - Specify the export settings').exists?
      #flag = Integer((@browser.span(:text=>'Export - Specify the export settings').id).gsub('_title','').gsub('uniqName_2_','')) #export dialog uniqName_2 id 
      #puts flag
      #id = flag*2-1    
      #puts id
      #select_id="dijit_form_Select_"+id.to_s               
       @browser.table(:id => 'input_export_container_type').send_keys(type1) 
    end 
    
  end
  
  #select a value for Metadata type.
  def selectMetadataType(type2)
    if @browser.span(:text=>'Export - Specify the export settings').exists?
       flag = Integer((@browser.span(:text=>'Export - Specify the export settings').id).gsub('_title','').gsub('uniqName_2_','')) #export dialog uniqName_2 id 
       id = flag*2     
       select_id="dijit_form_Select_"+id.to_s               
       @browser.table(:id => select_id).send_keys(type2) 
    end 
    
  end
  
  #input file location.
  def setFileLocation(flocation)
    if @browser.span(:text=>'Export - Specify the export settings').exists?
      #flag = Integer((@browser.span(:text=>'Export - Specify the export settings').id).gsub('_title','').gsub('uniqName_2_','')) #export dialog uniqName_2 id 
      #id = flag*2+1     #when uniqName_2 id is 1 ,inputID is 3
      #select_id="dijit_form_ValidationTextBox_"+id.to_s
       @browser.input(:id => 'input_export_location').wait_until_present
       @browser.input(:id => 'input_export_location').send_keys(flocation)
    end
  end
  
  #click on OK button.
  def clickOK
    deleteSure = BtnIDHelper.new(@browser)
    deleteSure.get_Dlg_btnID("Export - Specify the export settings","OK")   
  end
  
  
end