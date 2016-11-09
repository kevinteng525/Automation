class HelpController < ApplicationController
  # GET /helps
  def index
    @help_items = [["Overview","/assets/Galaxy Overview.pptx"],
                   ["Introduction","/assets/Galaxy Update 201-07-14.pptx"],
                   ["Architecture Design","/assets/Galaxy Architecture.vsd"],
                   ["Galaxy Status Transfer Chart","/assets/galaxy status transfer.vsd"],
                   ["Workflow of Job Manager","/assets/JobManagement.vsd"],
                   ["Workflow of Task Manager","/assets/TaskManagement.vsd"],
                   ["Workflow of Saber Agent","/assets/SaberAgent.vsd"],
                   ["Cancellation & Timeout of Task and Job","/assets/TimeOut & Cancel.vsd"],
                   ["Deployment Manual of Galaxy", "/assets/Deployment Manual of Galaxy Development Environment.docx"],
                   ["Environment Config Spec", "/assets/Environment Config Spec.docx"],
                   ["Introduction of Saber(S1 Common APIs Library)", "/assets/Saber Design.pptx"],
                   ["Ruby Minitest Support of Galaxy","/assets/Ruby Minitest Support.docx"],
                   ["Folder structures for automation testing scripts","/assets/Folder Structure of Test Scripts.docx"]]
  end
end
