class DiagnosticLogsController < ApplicationController
  before_action :set_diagnostic_log, only: [:show, :edit, :update, :destroy]

  # GET /diagnostic_logs
  # GET /diagnostic_logs.json
  def index
    @diagnostic_logs = DiagnosticLog.all
  end

  # GET /diagnostic_logs/1
  # GET /diagnostic_logs/1.json
  def show
  end

  # GET /diagnostic_logs/new
  def new
    @diagnostic_log = DiagnosticLog.new
  end

  # GET /diagnostic_logs/1/edit
  def edit
  end

  # POST /diagnostic_logs
  # POST /diagnostic_logs.json
  def create
    @diagnostic_log = DiagnosticLog.new(diagnostic_log_params)

    respond_to do |format|
      if @diagnostic_log.save
        format.html { redirect_to @diagnostic_log, notice: 'Diagnostic log was successfully created.' }
        format.json { render action: 'show', status: :created, location: @diagnostic_log }
      else
        format.html { render action: 'new' }
        format.json { render json: @diagnostic_log.errors, status: :unprocessable_entity }
      end
    end
  end

  # PATCH/PUT /diagnostic_logs/1
  # PATCH/PUT /diagnostic_logs/1.json
  def update
    respond_to do |format|
      if @diagnostic_log.update(diagnostic_log_params)
        format.html { redirect_to @diagnostic_log, notice: 'Diagnostic log was successfully updated.' }
        format.json { head :no_content }
      else
        format.html { render action: 'edit' }
        format.json { render json: @diagnostic_log.errors, status: :unprocessable_entity }
      end
    end
  end

  # DELETE /diagnostic_logs/1
  # DELETE /diagnostic_logs/1.json
  def destroy
    @diagnostic_log.destroy
    respond_to do |format|
      format.html { redirect_to diagnostic_logs_url }
      format.json { head :no_content }
    end
  end

  private
    # Use callbacks to share common setup or constraints between actions.
    def set_diagnostic_log
      @diagnostic_log = DiagnosticLog.find(params[:id])
    end

    # Never trust parameters from the scary internet, only allow the white list through.
    def diagnostic_log_params
      params.require(:diagnostic_log).permit(:create_time, :component, :log_type, :message)
    end
end
