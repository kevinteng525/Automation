require 'watir-webdriver'
require_relative '..\Variables\SUP_Variables_Configure.rb'
require_relative '..\Objects\Supervisor_Objects.rb'
require_relative '..\Objects\SUP_MessageList_Page.rb'
gem "minitest"
require "minitest/autorun"

class SUPPreviewPane < SUPMessageListPage
  
  #get the text content in preview pane.
  def getPreviewText(irow)
    if preview_text(irow.to_i).exists?
      return preview_text(irow.to_i).attribute_value("textContent").to_s
    else
      puts "Function 'getPreviewText' of SUP_Preview_Pane.rb - error: subject cannot be found in preview."
    end
  end
  
  #get the received date in preview.
  def getReceivedDate
    if preview_received_date_text.exists?
      received_date = preview_received_date_text.attribute_value("textContent").to_s
      date = getSubStr(received_date, "Received:")
      return date[2,255].strip
    else
      puts "Function 'getReceivedDate' of SUP_Preview_Pane.rb - error: received date cannot be found in preview."
    end    
  end
  
  #check the subject.
  def checkSubject(irow)  
    column_subject = getSubejctColumnText(irow)
    preview_subject = getPreviewText(3)
    puts"column_subject="+column_subject.to_s
    puts"preview_subject="+preview_subject.to_s
    if column_subject == preview_subject
      return true
    else
      puts "Function 'checkSubject' of SUP_Preview_Pane.rb - error: subject in column does not equal in preview pane."
      return false
    end
  end
  
  def checkMessageDate(irow)  
    #check the message date.
    column_date = getMessageDateColumnTextWithoutSecond(irow)
    date_preview = getReceivedDate
    #puts"colum_date="+colum_date.to_s
    puts"date_preview="+date_preview.to_s
    if column_date == date_preview
      return true
    else
      puts "Function 'checkMessageDate' of SUP_Preview_Pane.rb - error: message date in column does not equal in preview pane."
      return false
    end    
  end   
  
  def checkBcc
    str_bcc = getPreviewText(2)
    puts "str_bcc="+str_bcc.to_s
    if str_bcc.include?"allanecluff"
      return true
    else
      puts "Function 'checkBcc' of SUP_Preview_Pane.rb - error: BCC is not in preview pane."
      return false
    end
  end
  
  def checkPreviewContent(irow)
    #select one message.
    clickSpecifyMessageBody(irow)
    #check subject.
    result = checkSubject(irow).to_s
    #check message date.
    result += checkMessageDate(irow).to_s
    #check BCC.
    result += checkBcc.to_s
    if result.include?"false"
      return false
    else
      return true
    end
  end
  
  private

  #to, cc, bcc and subject in preview.
  def preview_text(irow)
    @browser.div(:id =>"MsgPreviewPane").div(:class =>"previewPane").td(:width =>"90%", :index =>irow)
  end
  
  #received date in preview.
  def preview_received_date_text
    @browser.div(:id =>"MsgPreviewPane").div(:class =>"previewPane").td(:align => "right")
  end
  
end